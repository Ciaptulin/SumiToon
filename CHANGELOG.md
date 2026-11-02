# Changelog

本项目遵循 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/) 格式。

## [Unreleased]

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

- AlePostProcessingURP 源码尚未验证编码（可能为 GBK）
- `_FloatWaveSpeedX/Y` 变量名与实际驱动方向相反（原作者历史命名，保留以兼容材质）
- MatCap 三种混合模式使用 3 个独立关键字，可合并优化变体
