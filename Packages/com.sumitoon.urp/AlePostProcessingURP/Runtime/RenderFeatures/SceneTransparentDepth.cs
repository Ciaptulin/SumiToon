// ============================================================
// 文件：SceneTransparentDepth.cs
// 用途：自定义 RendererFeature，把透明物体的深度渲染到 _CameraTransparentDepthTexture
//      供水面 Shader 采样（_CameraTransparentDepthTexture）
// 依赖：URP 16（UnityEngine.Rendering.Universal）、UnityEngine.Experimental.Rendering
// 作者：AleFeng（原作者）+ SumiToon 迁移到 URP 16    最后修改：2026-11-02
// 许可：MIT
// 说明：原版为 URP 14 编写，使用已废弃的 RenderTargetHandle；
//       本次迁移到 URP 16 的 RTHandle + OnCameraSetup 体系，
//       参考官方 CopyDepthPass 的纯 depth RT 做法。
// ============================================================
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

namespace SumiToon.PostProcessing
{
    /// <summary>
    /// 渲染透明物体深度到 _CameraTransparentDepthTexture，供水面 Shader 采样。
    /// </summary>
    [Serializable]
    public class SceneTransparentDepth : ScriptableRendererFeature
    {
        private TransparentDepthPass depthPass = null;

        /// <summary>
        /// 目标纹理名称（Shader 里通过该名称采样）
        /// </summary>
        private const string k_DepthTextureName = "_CameraTransparentDepthTexture";

        public override void Create()
        {
            depthPass = new TransparentDepthPass(
                RenderPassEvent.AfterRenderingPrePasses,
                RenderQueueRange.transparent,
                -1);
            depthPass.Setup(k_DepthTextureName);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(depthPass);
        }

        protected override void Dispose(bool disposing)
        {
            // 释放 RTHandle
            depthPass?.ReleaseResources();
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// 渲染所有带 'DepthOnly' pass 的透明物体到目标深度缓冲。
    /// 迁移说明：URP 16 起 ConfigureTarget 需要 RTHandle，
    ///           RT 通过 RenderingUtils.ReAllocateIfNeeded 分配，
    ///           纯 depth RT 的做法参考官方 CopyDepthPass。
    /// </summary>
    public class TransparentDepthPass : ScriptableRenderPass
    {
        int kDepthBufferBits = 32;

        private RTHandle destination;
        private string destinationName;

        FilteringSettings m_FilteringSettings;
        const string m_ProfilerTag = "Transparent Depth Prepass";
        ProfilingSampler m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);
        ShaderTagId m_ShaderTagId = new ShaderTagId("DepthOnly");

        public TransparentDepthPass(RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask)
        {
            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            renderPassEvent = evt;
        }

        public void Setup(string name)
        {
            this.destinationName = name;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            // URP 16 用 GraphicsFormat 指定深度格式；纯 depth RT 只设 graphicsFormat
            descriptor.graphicsFormat = GraphicsFormat.D32_SFloat_S8_UInt;
            descriptor.depthBufferBits = kDepthBufferBits;
            descriptor.msaaSamples = 1;

            RenderingUtils.ReAllocateIfNeeded(ref destination, descriptor, name: destinationName);

            if (destination != null)
            {
                // 纯 depth RT：color 和 depth attachment 都指向同一个 handle
                // 这是 URP 官方 CopyDepthPass 在 D3D11 下的做法
                ConfigureTarget(destination, destination);
                ConfigureClear(ClearFlag.All, Color.black);
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(m_ShaderTagId, ref renderingData, sortFlags);
                drawSettings.perObjectData = PerObjectData.None;

                ref CameraData cameraData = ref renderingData.cameraData;
                Camera camera = cameraData.camera;
                m_FilteringSettings.layerMask = camera.cullingMask;

                // URP 16：DrawRenderers 已过时，改用 RendererList API
                var rendererListParams = new RendererListParams(renderingData.cullResults, drawSettings, m_FilteringSettings);
                var rendererList = context.CreateRendererList(ref rendererListParams);
                cmd.DrawRendererList(rendererList);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // RT 由 RTHandleSystem 管理，跨 camera 复用，不在这里释放
        }

        /// <summary>
        /// 释放 RTHandle（由 Feature.Dispose 调用）
        /// </summary>
        public void ReleaseResources()
        {
            destination?.Release();
            destination = null;
        }
    }
}
