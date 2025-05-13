using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace TND.DLSS
{
    public class DLSSRenderPass : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [HideInInspector]
        public BoolParameter enable = new BoolParameter(false);
        public bool IsActive() => enable.value;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.BeforePostProcess;

        private readonly int depthTexturePropertyID = Shader.PropertyToID("_CameraDepthTexture");
        private readonly int motionTexturePropertyID = Shader.PropertyToID("_CameraMotionVectorsTexture");

        private DLSS_HDRP m_upscaler;
        private DLSS_Quality currentQuality;

        public override void Setup()
        {
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
#if TND_DLSS && UNITY_STANDALONE_WIN && UNITY_64

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (!IsActive())
            {
                cmd.Blit(source, destination, 0, 0);
                return;
            }

            if (camera.camera.cameraType != CameraType.Game)
            {
                cmd.Blit(source, destination, 0, 0);
                return;
            }
            if (m_upscaler == null && !camera.camera.TryGetComponent(out m_upscaler))
            {
                cmd.Blit(source, destination, 0, 0);
                return;
            }
            if (currentQuality != m_upscaler.DLSSQuality)
            {
                cmd.Blit(source, destination, 0, 0);
                currentQuality = m_upscaler.DLSSQuality;
                return;
            }

            m_upscaler.dlssCMD = cmd;
            m_upscaler.state.CreateContext(m_upscaler.dlssData, cmd, true);
            m_upscaler.state.UpdateDispatch(source, Shader.GetGlobalTexture(depthTexturePropertyID), Shader.GetGlobalTexture(motionTexturePropertyID), null, destination, cmd);
         
#endif
        }

        public override void Cleanup()
        {
        }
    }
}
