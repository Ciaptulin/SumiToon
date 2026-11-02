# SumiToon 工程化重构与底层排雷记录

> 本文档记录了本项目由 **Ciaptulin** 主导的工程重构与排错工作。

上游原代码是一个很好的 NPR 渲染研究项目，但作为工业化资产存在大量隐患：目录散装、废弃 API、内存泄漏与资产损坏。**我的核心工作，就是排掉这些雷，并把它重构为现代 Unity 规范包。**

## 一、底层排雷与疑难 Bug 修复
1. **修复后处理管线显存泄漏 (RT Leak)**
   - 定位并修复了 `CustomPostProcess.cs` 中因 `CommandBufferPool.Release` 早于 `ReleaseTemporaryRT` 导致的 RT 显存泄漏，重构了释放时序。
2. **抢救损坏的 Volume 序列化资产**
   - 深入 YAML 排查并修复了 4 个后处理配置文件的 `m_Script` 丢失与字段名（`blend` vs `m_Blend`）不匹配问题，挽救了所有配置数据。
3. **修复编辑器体验与渲染异常**
   - 修复了 Renderer 列表永久缓存导致的“新增效果必须重启 Editor”问题；修复了 Volume 组件强制覆盖用户勾选状态的逻辑；修复了 Scene 视图下后处理尺寸计算错误的缺陷。

## 二、URP 16 (Unity 2023.2) 深度迁移
- 彻底废弃并移除了 `RenderTargetHandle`，将 `SceneTransparentDepth.cs` 与 `CustomPostProcess.cs` 全面迁移至 URP 16 的 `RTHandle` 体系。
- 修复了 `ConfigureTarget`、`cameraColorTarget`、`useNativeRenderPass` 等底层 API 的编译崩溃问题。

## 三、包体迁移与工程化基建
- **UPM 重构**：将散装代码全量剥离至标准包 `Packages/com.sumitoon.urp`，清理了原作者的命名空间污染，统一为 `SumiToon.*`。
- **CI 自动化**：搭建 GitHub Actions，强制拦截废弃路径、JSON 格式与 Shader 规范问题。
- **测试保障**：编写 EditMode 测试，自动验证 Shader 的 Properties 与 CBUFFER 内存一致性。

---
*基础渲染数学逻辑源自 AleToonURP (MIT)。*
