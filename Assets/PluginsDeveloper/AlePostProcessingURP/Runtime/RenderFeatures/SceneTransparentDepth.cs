using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AlePostProcessUPR
{
    /// <summary>
    /// This renderer feature renders the depth of transparent objects to a texture named "_CameraTransparentDepthTexture"
    /// </summary>
    [Serializable]
    public class SceneTransparentDepth : ScriptableRendererFeature
    {
        
        /// <summary>
        /// The render pass
        /// </summary>
        private TransparentDepthPass depthPass = null;

        /// <summary>
        /// The render target ID for the scene transparent depth
        /// URP 16 起 RenderTargetHandle 已废弃，改用 Shader.PropertyToID + int
        /// </summary>
        private int sceneTransparentDepthTextureId;

        /// <summary>
        /// Intializes the renderer feature resources
        /// </summary>
        public override void Create()
        {
            depthPass = new TransparentDepthPass(RenderPassEvent.AfterRenderingPrePasses, RenderQueueRange.transparent, -1);
            sceneTransparentDepthTextureId = Shader.PropertyToID("_CameraTransparentDepthTexture");
        }

        /// <summary>
        /// Here you can inject one or multiple render passes in the renderer.
        /// This method is called when setting up the renderer once per-camera.
        /// </summary>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            depthPass.Setup(sceneTransparentDepthTextureId);
            renderer.EnqueuePass(depthPass);
        }
    }

    /// <summary>
    /// Render all transparent objects that have a 'DepthOnly' pass into the given depth buffer.
    ///
    /// You can use this pass to prime a depth buffer for subsequent rendering.
    /// Use it as a z-prepass, or use it to generate a depth buffer.
    /// 
    /// Code mostly copied from "DepthOnlyPass.cs" from the Universal Render Pipeline
    /// </summary>
    public class TransparentDepthPass : ScriptableRenderPass
    {
        int kDepthBufferBits = 32;

        private int destinationId = -1;  // -1 表示未设置（对应旧的 RenderTargetHandle.CameraTarget 语义）

        FilteringSettings m_FilteringSettings;
        const string m_ProfilerTag = "Transparent Depth Prepass";
        ProfilingSampler m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);
        ShaderTagId m_ShaderTagId = new ShaderTagId("DepthOnly");

        /// <summary>
        /// Create the TransparentDepthPass
        /// </summary>
        public TransparentDepthPass(RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask)
        {
            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            renderPassEvent = evt;
        }

        /// <summary>
        /// Configure the pass
        /// </summary>
        public void Setup(int destinationId)
        {
            this.destinationId = destinationId;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor descriptor = cameraTextureDescriptor;
            descriptor.colorFormat = RenderTextureFormat.Depth;
            descriptor.depthBufferBits = kDepthBufferBits;
            descriptor.msaaSamples = 1;

            cmd.GetTemporaryRT(destinationId, descriptor, FilterMode.Point);
            ConfigureTarget(new RenderTargetIdentifier(destinationId));
            ConfigureClear(ClearFlag.All, Color.black);
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(m_ShaderTagId, ref renderingData, sortFlags);
                drawSettings.perObjectData = PerObjectData.None;

                ref CameraData cameraData = ref renderingData.cameraData;
                Camera camera = cameraData.camera;
                
                m_FilteringSettings.layerMask = camera.cullingMask;

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings);

            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// <inheritdoc/>
        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");

            if (destinationId != -1)
            {
                cmd.ReleaseTemporaryRT(destinationId);
                destinationId = -1;
            }
        }
    }
}
