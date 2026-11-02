# Changelog — com.sumitoon.urp

本包遵循 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/) 与 [Semantic Versioning](https://semver.org/lang/zh-CN/)。

## [Unreleased]

## [0.1.0] - 2026-11-02

### Added

- Lit Shader：三色阶明暗、外描边、边缘光、高光、自发光、MatCap
- WaterPlane Shader：深度水色、多层波形、反射折射、边缘泡沫
- 自定义后处理框架（`CustomPostProcess` RendererFeature + `CustomVolumeComponent`）
- 14 种后处理效果：GaussianBlur / GaussianBlurGradient / RadialBlur / Mosaic / Zoom / Distort / DistortRipple / Ripple / ImageDizzy / ImageWaggle / EdgeMask / EdgeDetectionSobel / Grayscale / CyberFault / ViewportTrans
- 自定义 ShaderGUI：折叠式材质面板、多选材质批量编辑
- AleCameraRover 场景漫游相机组件
- Samples~/Demo 示例资源

### Fixed

- `CustomPostProcess` 临时 RT 释放在 CommandBuffer 回收之后执行，导致显存泄漏
- `GaussianBlur*` 用 `Screen.width` 计算 RT 尺寸，在 Scene 视图/多相机下尺寸错乱
- `CustomPostProcessRenderer.Setup` 首帧漏渲染、无法真正关闭效果
- `SceneTransparentDepth` 迁移到 URP 16 RendererList API
- ShaderGUI 的 GUI.color 异常路径不复原、折叠面板状态跨 Shader 串扰

### Known Issues

- 自定义后处理真正生效的开关是 `Enable` 参数的 overrideState，而非 Inspector 顶部的 `Active`
- `CustomPostProcessSettingsEditor` 的 Renderer 列表只在首次打开面板时扫描，新增需重启 Editor
