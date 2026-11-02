# SumiToon: 深度重构与技术排雷记录

> 本文档详细记录了本项目（SumiToon）是如何从上游基础代码（AleToonURP）被重构为工业级工程包的。
> 所有的底层重构、API 迁移、内存泄漏排查与基建搭建，均由 **Ciaptulin** 主导完成。

上游原项目是一个非常棒的 NPR 研究代码，但在作为工业化资产使用时，暴露出了大量的隐患：目录散装、废弃 API 报错、甚至存在内存泄漏和资产文件损坏等深水区 Bug。

**我的核心工作，就是排掉这些雷，并把它变成一套符合现代 Unity 规范的商业级渲染管线。**

---

## 一、底层排雷与 Bug 修复（核心价值）

这是本项目最具技术含金量的部分。在不破坏原作者渲染数学模型的前提下，我排查并修复了严重阻断工程的底层问题：

1. **修复渲染纹理 (RT) 内存泄漏**
   - **发现**：后处理管线在连续运行后显存持续飙升。
   - **排查**：定位到 `CustomPostProcess.cs` 中 `CommandBufferPool.Release` 的执行早于 `ReleaseTemporaryRT`，导致 RT 对象被池回收前永远无法释放。
   - **解决**：重构了 CommandBuffer 的释放时序，彻底解决了显存泄漏。
2. **抢救损坏的序列化资产 (Volume 配置)**
   - **发现**：场景中的 4 个后处理 Volume 配置文件在 Unity 中显示为 "Missing Script"，且修改的参数无法保存。
   - **排查**：深入 YAML 序列化文件，发现上游遗留了错误的 `m_Script` GUID，并且代码与资产的字段名（如 `blend` 与 `m_Blend`）不一致。
   - **解决**：手工重构了 YAML 结构，回填了正确的 GUID 并统一了字段名，挽救了所有后处理参数。
3. **修复编辑器体验缺陷与强制覆盖**
   - 解决了 `CustomPostProcessSettingsEditor` 永久缓存导致“新增渲染器必须重启 Editor”的严重体验问题。
   - 修复了 Volume 组件每次重载时强制覆写用户勾选状态（`overrideState`）的逻辑，将控制权还给美术。
4. **修复 Scene 视图渲染异常**
   - 修正了高斯模糊等后处理在 Scene 视图或非原生分辨率下计算错误的问题（将硬编码的 `Screen.width` 替换为响应式的 `cameraTargetDescriptor`）。

## 二、URP 16 (Unity 2023.2) 深度兼容迁移

原代码依赖 URP 14，在 Unity 2023 中会直接触发编译崩溃。我完成了彻底的图形 API 迁移：
- **RT 句柄革命**：彻底废弃了 `RenderTargetHandle`，重写了 `SceneTransparentDepth.cs` 和 `CustomPostProcess.cs`，全面拥抱 `RTHandle` 与 `RTHandles.Alloc` 体系。
- **管线上下文适配**：修复了 `ConfigureTarget(RenderTargetIdentifier)` 与 `cameraColorTarget` 等废弃 API 的报错，利用隐式转换确保渲染管线正常运作。
- **权限与编译隔离**：修复了外部程序集无法访问 internal 级 `useNativeRenderPass` 导致的编译中断问题。

## 三、架构级重构与工程化基建

把烂摊子整理成能发给客户的标准件：

- **UPM 包体迁移**：将原本散落在 `Assets/PluginsDeveloper` 下的代码，完整剥离并重构成了规范化的 `Packages/com.sumitoon.urp`。包含 `package.json`、文档、Samples 与依赖管理。
- **命名空间清洗**：将原作者带私货前缀的命名空间（`AleToonURP` / `AlePostProcessURP`）全量清洗为 `SumiToon.*`，并重构了 5 个 `.asmdef` 实现程序集隔离。
- **自动化 CI 防护**：从零搭建 GitHub Actions（`ci.yml`），强制校验废弃路径、JSON 格式、asmdef 命名及 Shader 文件头，杜绝后续开发的低级错误。
- **测试保障**：编写了 EditMode 测试用例，自动验证 Shader Properties 与 CBUFFER 的内存对齐，防止改崩渲染。

---

*本文档仅列举了工程层面的核心重构。原作者的 Shader 渲染数学与美术表现逻辑在重构中被妥善保留，并在 MIT 协议的许可下继续发光发热。*
