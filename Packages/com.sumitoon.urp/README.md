# SumiToon URP

基于 [AleToonURP](https://github.com/AleFeng/AleToonURP) 的 Unity URP NPR（二次元卡通）渲染管线。

## 安装

### 方式一：Git URL

Package Manager → **+** → **Add package from git URL**：

```
https://github.com/SumiToon/SumiToon.git?path=/Packages/com.sumitoon.urp#v0.1.0
```

### 方式二：本地路径

Package Manager → **+** → **Add package from disk**，选择本目录的 `package.json`。

## 环境要求

- Unity 2023.2.20f1 或更高
- URP 16.0.6
- 依赖包 `com.alefeng.outlinesmoothnormalsgenerator`（描边平滑法线生成器）

## 包含内容

| 模块 | 说明 |
|---|---|
| `Runtime/Shaders/` | Lit / WaterPlane Shader 与 HLSL |
| `Runtime/` | 运行时程序集（`SumiToon.Runtime`） |
| `Editor/` | 自定义 ShaderGUI（`SumiToon.Editor`） |
| `AlePostProcessingURP/` | 自定义后处理框架与 14 种效果 |
| `AleCameraRover/` | 场景漫游相机组件 |
| `Samples~/Demo/` | 示例材质与贴图（需在 Package Manager 中手动 Import） |
| `Documentation~/` | 用户手册（含参数说明与截图） |

## 文档

- [用户手册](./Documentation~/UserManual.md) — 每个 Shader 参数的详细说明
- [CHANGELOG](./CHANGELOG.md) — 版本变更记录

## 许可

代码部分采用 MIT License。示例模型资产的版权归其原作者所有，仅用于学习，不得商用。详见仓库根目录 `THIRD-PARTY-NOTICES.md`。
