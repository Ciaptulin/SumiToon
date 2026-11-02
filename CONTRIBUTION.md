# 贡献记录与开发历程 (Development History)

本项目基于 AleToonURP 改造而来。在这个仓库中，本人完成了从**“学习源码”到“发布商业化 UPM 包”**的全部重构、排错与规范制定工作。

## 核心贡献说明

### 1. 学习与源码重构 (Learning & Refactoring)
- **手敲核心源码**：为深入理解 NPR 渲染原理，手敲了 `Function.hlsl` / `LitMain.hlsl` / `LitOutline.hlsl` / `LitPropInput.hlsl` 等数百行核心逻辑（保留在 `Learning/handwritten/` 目录）。
- **UPM 包迁移**：将原本散落在 `Assets/PluginsDeveloper` 下的代码，完整迁移并重构为符合 Unity 官方规范的 UPM 包 `Packages/com.sumitoon.urp`。
- **命名空间规范**：彻底清理原作者前缀（`AleToonURP` / `AlePostProcessURP`），全局统一为 `SumiToon.*` 命名空间与程序集。

### 2. URP 16 兼容性排查与修复 (URP 16 Migration)
在将项目升级到 Unity 2023.2 / URP 16 时，排查并修复了大量底层 API 废弃问题：
- 修复 `SceneTransparentDepth.cs` 和 `CustomPostProcess.cs` 中废弃的 `RenderTargetHandle`，迁移至 `RTHandle`。
- 修复 `ScriptableRenderPass.ConfigureTarget` 和 `cameraColorTarget` 的废弃报错。
- 修复 `useNativeRenderPass` 为 internal 导致的编译报错。

### 3. 深水区 Bug 修复 (Deep Bug Fixing)
- **Volume 资产修复**：排查出 Volume 序列化资产里 `m_Script` 丢失、字段名（`blend` vs `m_Blend`）不匹配的深水区 bug，挽救了所有后处理配置。
- **显存泄漏修复**：修复了 `CustomPostProcess.cs` 中 RT 释放在 CommandBuffer 之后导致显存泄漏的问题。
- **Renderer 列表刷新**：修复了 `CustomPostProcessSettingsEditor` 里永久缓存导致新增 Renderer 必须重启 Editor 的体验问题。
- **Scene 视图兼容**：修复了高斯模糊等后处理在 Scene 视图中尺寸错误的问题。

### 4. 工程化与合规 (Engineering & Compliance)
- **CI 自动化**：编写了 GitHub Actions (`ci.yml`)，自动检查废弃路径、JSON 合法性、asmdef 命名规范、Shader 文件头以及 LFS 大文件。
- **测试用例**：编写了 EditMode 测试（`ShaderPropertyConsistencyTests`），保证 Shader Properties 和 CBUFFER 的一致性。
- **文档与合规**：补全了全量工程文档（README、GETTING_STARTED、ARCHITECTURE 等），并处理了 MIT 协议的 THIRD-PARTY-NOTICES。

---

*详细的代码修改过程，请参考本仓库的 Git Commit 记录。*
