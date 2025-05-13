using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;
using UnityEngine.Experimental.Rendering;

#if UNITY_6000_0_OR_NEWER
using System;
using UnityEngine.Rendering.RenderGraphModule;
#endif

namespace TND.DLSS
{
    public class DLSSRenderPass : ScriptableRenderPass
    {
        private DLSS_URP m_upscaler;
        private const string blitPass = "[DLSS] Upscaler";

        //Legacy
        private Vector4 _scaleBias;

        public DLSSRenderPass(DLSS_URP _upscaler, bool usingRenderGraph)
        {
            m_upscaler = _upscaler;
            renderPassEvent = usingRenderGraph ? RenderPassEvent.AfterRenderingPostProcessing : RenderPassEvent.AfterRendering + 2;

            _scaleBias = SystemInfo.graphicsUVStartsAtTop ? new Vector4(1, -1, 0, 1) : Vector2.one;
        }

        #region Unity 6

#if UNITY_2023_3_OR_NEWER
        private class PassData
        {
            public TextureHandle Source;
            public TextureHandle Depth;
            public TextureHandle MotionVector;
            public TextureHandle Destination;
            public GraphicsFormat format;
            public Rect PixelRect;
        }

        private int multipassId = 0;
        private const string _upscaledTextureName = "_DLSS_UpscaledTexture";

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
#if TND_DLSS && UNITY_STANDALONE_WIN && UNITY_64
            // Setting up the render pass in RenderGraph
            using (var builder = renderGraph.AddUnsafePass<PassData>(blitPass, out var passData))
            {
                var cameraData = frameData.Get<UniversalCameraData>();
                var resourceData = frameData.Get<UniversalResourceData>();

                RenderTextureDescriptor upscaledDesc = cameraData.cameraTargetDescriptor;
                upscaledDesc.depthBufferBits = 0;
                upscaledDesc.width = m_upscaler.m_displayWidth;
                upscaledDesc.height = m_upscaler.m_displayHeight;

                TextureHandle upscaled = UniversalRenderer.CreateRenderGraphTexture(
                    renderGraph,
                    upscaledDesc,
                    _upscaledTextureName,
                    false
                );

                passData.Source = resourceData.activeColorTexture;
                passData.Depth = resourceData.activeDepthTexture;
                passData.MotionVector = resourceData.motionVectorColor;
                passData.Destination = upscaled;
                passData.PixelRect = cameraData.camera.pixelRect;
                passData.format = cameraData.cameraTargetDescriptor.graphicsFormat;

                builder.UseTexture(passData.Source, AccessFlags.Read);
                builder.UseTexture(passData.Depth, AccessFlags.Read);
                builder.UseTexture(passData.MotionVector, AccessFlags.Read);
                builder.UseTexture(passData.Destination, AccessFlags.Write);

                builder.AllowPassCulling(false);

                resourceData.cameraColor = upscaled;
                builder.SetRenderFunc((PassData data, UnsafeGraphContext context) => ExecutePass(data, context));
            }
#endif
        }

        private void ExecutePass(PassData data, UnsafeGraphContext context)
        {
#if TND_DLSS && UNITY_STANDALONE_WIN && UNITY_64
            CommandBuffer unsafeCmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
            m_upscaler.dlssCMD = unsafeCmd;


            //Stereo
            if (XRSettings.enabled)
            {
                multipassId++;
                if (multipassId >= 2)
                {
                    multipassId = 0;
                }
            }
            m_upscaler.m_colorBuffer = data.Source;
            m_upscaler.m_depthBuffer = data.Depth;
            m_upscaler.m_motionVectorBuffer = data.MotionVector;

            m_upscaler.CameraGraphicsOutput = data.format;

            m_upscaler.state[multipassId].CreateContext(m_upscaler.dlssData, unsafeCmd, true);
            m_upscaler.state[multipassId].UpdateDispatch(m_upscaler.m_colorBuffer, m_upscaler.m_depthBuffer, m_upscaler.m_motionVectorBuffer, null, m_upscaler.m_dlssOutput, unsafeCmd);

            CoreUtils.SetRenderTarget(unsafeCmd, data.Destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, ClearFlag.None, Color.clear);
            unsafeCmd.SetViewport(data.PixelRect);

            Blitter.BlitTexture(unsafeCmd, m_upscaler.m_dlssOutput, new Vector4(1, 1, 0, 0), 0, false);
#endif
        }
#endif

        #endregion

        #region Unity Legacy
#if UNITY_6000_0_OR_NEWER
        [Obsolete]
#endif
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
#if TND_DLSS && UNITY_STANDALONE_WIN && UNITY_64
            try
            {
                CommandBuffer cmd = CommandBufferPool.Get(blitPass);
                CoreUtils.SetRenderTarget(cmd, BuiltinRenderTextureType.CameraTarget, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, ClearFlag.None, Color.clear);
                cmd.SetViewport(renderingData.cameraData.camera.pixelRect);

                if (renderingData.cameraData.camera.targetTexture != null)
                {
                    _scaleBias = Vector2.one;
                }

                Blitter.BlitTexture(cmd, m_upscaler.m_dlssOutput, _scaleBias, 0, false);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
            catch { }
#endif
        }
    }

    public class DLSSBufferPass : ScriptableRenderPass
    {
        private DLSS_URP m_upscaler;
        private CommandBuffer cmd;
        private const string blitPass = "[DLSS] Upscaler";
        private int multipassId = 0;

        private readonly int depthTexturePropertyID = Shader.PropertyToID("_CameraDepthTexture");
        private readonly int motionTexturePropertyID = Shader.PropertyToID("_MotionVectorTexture");

        public DLSSBufferPass(DLSS_URP _upscaler)
        {
            m_upscaler = _upscaler;

            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Motion);
        }

#if UNITY_6000_0_OR_NEWER
        [Obsolete]
#endif
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
#if TND_DLSS && UNITY_STANDALONE_WIN && UNITY_64

            m_upscaler.dlssCMD = cmd = CommandBufferPool.Get(blitPass);

#if UNITY_2022_1_OR_NEWER
            m_upscaler.m_colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
            m_upscaler.m_depthBuffer = Shader.GetGlobalTexture(depthTexturePropertyID);
            m_upscaler.m_motionVectorBuffer = Shader.GetGlobalTexture(motionTexturePropertyID);
#else
            Blit(cmd, renderingData.cameraData.renderer.cameraColorTarget, m_upscaler.m_colorBuffer);
            m_upscaler.m_depthBuffer = Shader.GetGlobalTexture(depthTexturePropertyID);
            m_upscaler.m_motionVectorBuffer = Shader.GetGlobalTexture(motionTexturePropertyID);
#endif

            //Stereo
            if (XRSettings.enabled)
            {
                multipassId++;
                if (multipassId >= 2)
                {
                    multipassId = 0;
                }
            }

            m_upscaler.CameraGraphicsOutput = renderingData.cameraData.cameraTargetDescriptor.graphicsFormat;
            m_upscaler.state[multipassId].CreateContext(m_upscaler.dlssData, cmd, true);
            m_upscaler.state[multipassId].UpdateDispatch(m_upscaler.m_colorBuffer, m_upscaler.m_depthBuffer, m_upscaler.m_motionVectorBuffer, null, m_upscaler.m_dlssOutput, cmd);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

#endif
        }
        #endregion
    }
}
