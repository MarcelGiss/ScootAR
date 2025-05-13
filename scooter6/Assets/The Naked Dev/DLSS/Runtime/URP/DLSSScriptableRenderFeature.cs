using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TND.DLSS
{
    public class DLSSScriptableRenderFeature : ScriptableRendererFeature
    {
        [HideInInspector]
        public bool IsEnabled = false;
        private bool usingRenderGraph = false;

        private DLSS_URP m_upscaler;

        private DLSSBufferPass _bufferPass;
        private DLSSRenderPass _renderPass;

        private CameraData cameraData;

        public override void Create()
        {
            name = "DLSSRenderFeature";

            SetupPasses();
        }

        public void OnSetReference(DLSS_URP _upscaler)
        {
            m_upscaler = _upscaler;
            SetupPasses();
        }

        private void SetupPasses()
        {
#if UNITY_2023_3_OR_NEWER
            var renderGraphSettings = GraphicsSettings.GetRenderPipelineSettings<RenderGraphSettings>();
            usingRenderGraph = !renderGraphSettings.enableRenderCompatibilityMode;
#endif

            if (!usingRenderGraph)
            {
                _bufferPass = new DLSSBufferPass(m_upscaler);
                _bufferPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Motion);
            }

            _renderPass = new DLSSRenderPass(m_upscaler, usingRenderGraph);
            _renderPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Motion);
        }


        public void OnDispose()
        {
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (!IsEnabled)
            {
                return;
            }

            cameraData = renderingData.cameraData;
            if (cameraData.camera != m_upscaler.m_mainCamera)
            {
                return;
            }
            if (!cameraData.resolveFinalTarget)
            {
                return;
            }

            // Here you can queue up multiple passes after each other.
            if (!usingRenderGraph)
            {
                renderer.EnqueuePass(_bufferPass);
            }
            renderer.EnqueuePass(_renderPass);
        }
    }
}
