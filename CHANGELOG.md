# Changelog

本项目遵循 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/) 格式。

## [Unreleased]

### Fixed

**AlePostProcessingURP 运行时问题（代码审查后修复）**

- `CustomPostProcess.cs`：临时 RT 释放在 `CommandBufferPool.Release` 之后执行，导致 `ReleaseTemporaryRT` 永远不生效、显存持续泄漏。改为先释放 RT 再执行/回收 CommandBuffer
- `GaussianBlurEffect.cs` / `GaussianBlurGradientEffect.cs`：RT 尺寸与模糊半径分母使用 `Screen.width/height`，在 Scene 视图、多相机、改 renderScale 时与实际渲染尺寸不一致。改用 `cameraTargetDescriptor`
- `CustomPostProcessRenderer.Setup()`：首帧 `m_VolumeComponent` 尚未初始化导致漏渲染；且仅用 `Enable.overrideState` 判断，用户无法真正关闭效果。改为主动初始化 + `overrideState && value` 判断
- `CustomPostProcessRenderer.GetVolumeComponent()`：类型不匹配时静默返回 null，改为类型不符时重新获取
- `CustomVolumeComponent.OnEnable()`：每次 Volume 栈重建都强制写 `Enable.overrideState`，用户无法取消该 override。改为仅实例首次启用时设置
- `GaussianBlurEffect.cs` 等 4 个文件：移除未使用的 `using static Unity.VisualScripting.Member;`（包级依赖泄漏）

**AleCameraRover**

- 光标可见/锁定状态每帧无条件写入，会与场景中其它光标控制冲突。改为状态变化时才写
- 旋转用 `eulerAngles` 读回再拼装，俯仰接近 ±90° 时角度跳变。改用 `Quaternion` 累积
- 删除未使用的 `speedChange` 变量与无标注 TODO
- 清理未使用的 `using`

**ShaderGUI（编辑器）**

- `OnBtnToggle` / `BtnToggleLabelPass`：GUI.color 在异常路径下不复原，会污染后续控件。改用 try/finally
- `BtnToggleLabelPass`：空材质数组时提前 `return` 漏掉 `EndHorizontal`，导致布局错乱
- 折叠面板状态字典用标题作 Key，不同 Shader 的面板共享展开状态。增加 Shader 名前缀隔离

### Added

- `docs/GETTING_STARTED.md`：新手入门（10 分钟从 clone 到看到效果，含 5 项验证清单）
- `docs/ARCHITECTURE.md`：架构与代码阅读指南（目录职责、Shader 文件关系、LitMain 阅读路线、数据流）
- `docs/TROUBLESHOOTING.md`：常见问题（9 类现象：材质粉红、场景黑、白模、描边、后处理、Game 视图发糊、乱码、占位符）
- `docs/CONTRIBUTING.md`：贡献指南（提交规范、代码规范、变体管理、Git 流程）
- `Document~/UserManual.md`：从 AleToonURP 原项目搬入的用户手册（含 30+ 张参数截图）
- `Learning/notes/`：学习笔记（Shader 代码规范、AleToonURP 代码审查报告、面试复习手册）
- README 增加"文档索引"章节

## [0.1.0] - 2026-11-02

### Added

- 引入 AleToonURP v1.4.0 原版（Lit / WaterPlane / Editor / Example）
- 引入 AleCameraRover 相机组件
- 引入 AlePostProcessingURP 自定义后处理
- 引入 ProductAssets 学习用模型与场景（DemoKafka / DemoKlee / DemoWater）
- 引入 URPSettings 管线配置与 Volume
- 建立 `Learning/` 学习记录目录，保存手敲的 4 个核心 HLSL
- 建立 Git 仓库，配置 LFS / .gitignore / .gitattributes / .editorconfig

### Changed

- **代码规范化**（仅格式，未改逻辑）：
  - 8 个 Shader/HLSL 文件补全文件头（用途 / 依赖 / 作者 / 许可）
  - Unicode 分区框（86 处）替换为 ASCII，避免跨编码乱码
  - 关键魔法数字补充"为什么"注释（10 处）
  - `3.141592654` 替换为 URP 内置 `PI` 常量（避免重定义）
- `ProjectSettings/GraphicsSettings.asset` 管线切换到 `AleToonUPR_Settings`
- `Packages/manifest.json` 新增 `com.alefeng.outlinesmoothnormalsgenerator` 依赖

### Fixed

- 首次搬运时的 UTF-8 / GBK 编码混用导致 `Function.hlsl` 中文注释乱码（已修复）
- 部分资产的 `.meta` 缺失导致 GUID 引用断开（已补齐）
- 目录误复制到项目根目录（已清理）

### Known Issues

- `_FloatWaveSpeedX/Y` 变量名与实际驱动方向相反（原作者历史命名，保留以兼容材质）
- MatCap 三种混合模式使用 3 个独立关键字，可合并优化变体
- 自定义后处理真正生效的开关是 `Enable` 参数的 overrideState，而非 Inspector 顶部的 `Active`（URP 的 `VolumeComponent.active` 有已知 Bug），容易误判，详见 `docs/TROUBLESHOOTING.md` 第 6 节
- `CustomPostProcessSettingsEditor` 的可用 Renderer 列表只在首次打开面板时扫描，新增 Renderer 需重启 Editor
- 命名空间 `AlePostProcessUPR` 大小写与文件头注释不一致（应为 URP），待统一重命名
- `SumiToonRuntimeInfo` 的 `ProjectUrl` 仍为占位符，且当前无任何引用
