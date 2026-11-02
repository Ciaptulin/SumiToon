# SumiToon 工程化重构与底层排雷记录

> 本文档记录了本项目由 **Ciaptulin** 主导的工程重构与排错工作。所有内容均对应代码中的真实 Diff 差异。

上游原代码是一个很好的 NPR 渲染研究项目，但在实际工业化和 URP 高版本升级时，暴露出了大量严重的底层隐患。**我的核心工作，就是排掉这些雷，并把它重构为现代 Unity 规范包。**

## 一、核心排雷：疑难 Bug 修复

1. **修复后处理管线致命显存泄漏 (RT Leak)**
   - **排查与修复**：原版代码中，临时渲染纹理（RT）的释放 (`ReleaseTemporaryRT`) 被错误地放置在了 `CommandBufferPool.Release` 之后。这导致命令写入已回收的 CommandBuffer，RT 永远无法释放，长时间运行会导致显存爆满（OOM）。
   - **代码级操作**：我将释放逻辑提前至 `context.ExecuteCommandBuffer` 之前，彻底切断了显存泄漏的根源。
2. **抢救损坏的 Volume 序列化资产**
   - **排查与修复**：原项目中有 4 个后处理配置（Volume）资产的序列化数据损坏，表现为 `m_Script` GUID 丢失或代码与资产的字段名（如 `blend` 与 `m_Blend`）不匹配，导致面板报 "Missing Script" 且调参无效。
   - **代码级操作**：深入 YAML 底层手工重构了 GUID 引用并统一了字段名，挽救了所有后处理参数。
3. **修复编辑器体验与渲染异常**
   - 移除了 `CustomPostProcessSettingsEditor` 中的永久缓存（原版导致新增渲染器必须重启 Editor 才能刷新）。
   - 修复了后处理在 Scene 视图或非原生分辨率下计算错误的问题（将硬编码的 `Screen.width` 替换为 `cameraTargetDescriptor`）。

## 二、底层 API 深度迁移 (URP 14 -> URP 16)
原代码在 Unity 2023.2 / URP 16 中完全无法编译，我进行了底层的图形 API 翻新：
- **RenderTargetHandle 彻底移除**：将 `SceneTransparentDepth.cs` (改动超过 120 行) 和 `CustomPostProcess.cs` 全面迁移至 `RTHandle` 与 `int + RenderTargetIdentifier` 体系。
- **废弃 API 适配**：修复了 `ConfigureTarget`、`cameraColorTarget`、`useNativeRenderPass` 等导致外部程序集编译崩溃的问题。

## 三、架构级重构与包体迁移
- **UPM 包体重构**：将散装代码全量剥离至标准包 `Packages/com.sumitoon.urp`，清理了原作者的命名空间污染，统一为 `SumiToon.*` 并重写了 5 个 `.asmdef`。
- **CI 与测试基建**：从零搭建 GitHub Actions，拦截废弃路径与格式错误；编写 EditMode 测试，保证 Shader 的 Properties 与 CBUFFER 内存布局一致性。

---
*基础渲染数学模型源自 AleToonURP (MIT 协议)，本仓库在此之上完成了全部的底层修复与架构重构。*
