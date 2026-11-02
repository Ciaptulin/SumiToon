# SumiToon: 深度重构与技术排雷说明

本项目是基于 `AleToonURP`（原作者：AleFeng）的深度重构版本。

原项目是一个很好的 NPR 渲染基础研究代码，但作为工业化工程仍存在大量隐患：目录结构散装、使用已废弃的 URP 14 API、缺乏 CI 防护、存在内存泄漏与序列化 Bug。

**本人接手后，将其从“跑得通的基础 Demo”彻底改造为了“符合现代 Unity UPM 规范的商业化级渲染包”。** 以下是本人主导的核心技术贡献：

---

## 核心贡献一：包体架构重构与 UPM 标准化
- **UPM 包体迁移**：摒弃了原生散落在 `Assets/` 下的非规范结构，将核心代码、Editor 工具、测试、文档完整迁移至标准 UPM 包目录 `Packages/com.sumitoon.urp`。
- **命名空间与程序集洗牌**：清理了原作者遗留的命名空间（`AleToonURP` / `AlePostProcessURP`），全局重构为 `SumiToon.*`，并重写了 5 个 `.asmdef` 以保证编译隔离与包体一致性。
- **依赖剥离与解耦**：将原本混在项目里的第三方依赖（如 `OutlineSmoothNormalsGenerator`）解耦为标准包依赖，避免代码耦合。

## 核心贡献二：URP 16 (Unity 2023.2) 深度迁移
原项目基于 URP 14 编写，在升级至 URP 16 时触发了大量底层 API 崩溃。本人完成了底层图形 API 的彻底迁移：
- **RT 句柄迁移**：彻底移除已废弃的 `RenderTargetHandle`，将 `SceneTransparentDepth.cs` 与 `CustomPostProcess.cs` 迁移至 URP 16 标准 `RTHandle` 体系。
- **渲染管线 API 适配**：修复了 `ConfigureTarget` 与 `cameraColorTarget` 的废弃报错，并巧妙利用隐式转换避开了连锁报错。
- **渲染上下文适配**：修复了外部程序集无法访问 `useNativeRenderPass` 导致的编译错误。

## 核心贡献三：深水区 Bug 排查与修复 (核心价值)
在工程化过程中，排查并修复了原作者隐藏的大量疑难杂症：
- **修复 RT 内存泄漏**：查明并修复了 `CustomPostProcess.cs` 中 `CommandBufferPool.Release` 早于 `ReleaseTemporaryRT` 执行导致的临时渲染纹理显存持续泄漏问题。
- **修复序列化资产损坏**：排查出 4 个 Volume 资产（后处理配置文件）中 `m_Script` 丢失及字段名（`blend` vs `m_Blend`）不匹配的问题，挽救了所有后处理调参数据。
- **修复编辑器体验缺陷**：解决了 `CustomPostProcessSettingsEditor` 永久缓存导致的“新增渲染器必须重启 Editor”的体验问题；并修复了参数强制覆盖用户勾选状态的问题。
- **修复 Scene 视图渲染异常**：修正了高斯模糊等后处理在 Scene 视图或非原生分辨率下计算错误的问题（将硬编码的 `Screen.width` 替换为 `cameraTargetDescriptor`）。

## 核心贡献四：工程化基建与质量保障
- **自动化 CI 体系**：从零搭建了 GitHub Actions（`.github/workflows/ci.yml`），实现了代码路径合规、JSON 格式、asmdef 命名规范及 Shader 文件头的自动化拦截。
- **自动化测试**：编写了 EditMode 测试用例（`ShaderPropertyConsistencyTests`），确保 Shader 的 Properties 声明与 CBUFFER 内存布局保持一致。
- **规范化文档**：编写并完善了全量工程文档（README、GETTING_STARTED、ARCHITECTURE、TROUBLESHOOTING），制定了严格的代码规范（`CONTRIBUTING.md`），并对所有 Shader 补充了文件头。

---

*注：本项目基础渲染逻辑源自 AleToonURP（MIT 协议）。在保留原作者渲染数学与 Shader 架构的前提下，本人完成了上述所有的工程级重构、API 兼容性修复与底层 Bug 排雷。*
