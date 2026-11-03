# SumiToon

**SumiToon** 是一个面向 Unity URP (Universal Render Pipeline) 的工业级 NPR（二次元卡通）渲染管线与 Shader 解决方案。

本项目最初起源于 AleToonURP（MIT 协议），并在本仓库中完成了**从基础研究 Demo 到商业化工程包的彻底重构**。我们不仅修复了原版底层隐患（URP 16 API 迁移、内存泄漏、资产序列化损坏），还在此基础上**深度扩展了面部 SDF 阴影系统，并成功落地了重度二次元角色（洛茜）的工业化接入**。

## 📖 目录
- [✨ 核心特性](#-核心特性)
- [🚀 实战案例：洛茜 (Rossi) 接入与管线扩展](#-实战案例洛茜-rossi-接入与管线扩展)
- [📦 安装方式](#-安装方式)
- [📂 目录结构](#-目录结构)
- [🛠️ 开发规范](#️-开发规范)
- [📄 许可](#-许可)

---

## ✨ 核心特性

### 底层架构重构 (继承自上游并修复)
- **彻底修复底层内存泄漏**：重构了后处理管线的 RT 释放逻辑，避免了长时间运行的显存溢出。
- **现代化 UPM 工程规范**：摒弃散装脚本，全量迁移为规范的 UPM 包（`com.sumitoon.urp`），开箱即用。
- **无缝升级 URP 16**：彻底移除了 `RenderTargetHandle` 等废弃 API，全面拥抱 `RTHandle` 体系，兼容 Unity 2023.2+。
- **抢救损坏的后处理资产**：修复了上游版本遗留的 Volume 资产序列化丢失与字段不匹配问题。
- **工业化 CI 与测试**：内置 GitHub Actions 自动化校验与 EditMode 测试，保证代码规范与 Shader 内存布局一致性。

### 渲染特性 (NPR 卡通管线)
- **NPR 卡通渲染 Lit Shader**：三色阶明暗、外描边、边缘光、高光、自发光、MatCap。
- **NPR 风格化水体 (Stylized Water)**：基于深度的浅水深海色阶、双层法线扰动、硬边碎光高光、噪波打碎的岸边泡沫，实现二次元“清澈见底”的视觉表现。
- **风吹草地顶点动画 (Wind Grass)**：基于世界坐标采样的双层噪声阵风，摆脱死板的集体摆动，让植被呈现自然的错落起伏。
- **UberShader**：通过宏开关生成变体，按需启用效果。
- **自定义 ShaderGUI**：折叠式材质面板，支持多选编辑与关键字实时生效。
- **SRP Batcher 兼容**：CBUFFER 按 UnityPerMaterial 组织，极致优化 DrawCall。

---

## 🚀 实战案例：洛茜 (Rossi) 接入与管线扩展

本项目不仅是一个 Shader 包，更经过了重度二次元角色（《明日方舟：终末地》洛茜）的工业化落地验证。在此过程中，针对实际美术需求对管线进行了深度扩展：

### 1. 面部 SDF 阴影系统 (FaceSDF) 跨引擎重构
- **跨引擎数据迁移**：将 Blender 中复杂的 3x3 图集面部阴影逻辑（9帧差分 + 阈值图）完整迁移至 Unity URP。
- **解决左右不对称与死黑**：重写 `LitFaceSDF.hlsl`，通过本地空间光照方向动态选择正/负图集，结合平滑阈值，彻底解决跨引擎带来的 UV 翻转错位、背光死黑、左右脸阴影不对称等问题。
- **光照解耦驱动**：新增 `FaceSDFLightDriver.cs`，实时将主光方向转换至模型本地空间，确保角色旋转时面部阴影依然精准跟随主光。

### 2. Shader 与 GUI 扩展
- **新增属性与面板**：在 `Lit.shader` 中增加 SDF 阴影专属属性，并修改 `ShaderGUILit.cs` 实现自定义折叠面板，支持调试帧与阴影阈值实时预览。
- **MMD 骨骼与表情适配**：新增 `MmdBlendShapeController.cs`，完美驱动 41 个日文命名 BlendShape（眨眼、口型），并支持自动呼吸/眨眼循环。

### 3. 资产管线与场景工业化落地
- **FBX 导出优化**：剥离冗余几何节点，修复金属重叠面，完成 514 骨 MMD 骨架与 5 套细节网格的精确蒙皮导出。
- **材质自动化装配**：编写 Editor 工具，实现 20 个材质的自动生成、贴图绑定、透明裁剪（Dither）及关键词一键配置。
- **渲染管线清理**：移除 Volume 中冗余的测试后处理，建立标准的 Tonemapping + Bloom + 颜色校正 二次元渲染流程。
- **环境氛围整合**：结合自定义的风格化水体与风吹草地顶点动画，打造出完整的可交互二次元演示场景。

---

## 📦 安装方式

在 Unity 中打开 `Window → Package Manager`，点击 `+ → Add package from git URL...`，输入：

```
https://github.com/Ciaptulin/SumiToon.git?path=/Packages/com.sumitoon.urp
```

### 环境要求
- **Unity**：2023.2.20f1
- **URP**：16.0.6
- **Git LFS**：必须安装（资产走 LFS）

### 完整项目克隆
```bash
git clone https://github.com/Ciaptulin/SumiToon.git
cd SumiToon
git lfs install
git lfs pull
```
用 Unity 2023.2.20f1 打开项目，等待首次导入完成（约 3~5 分钟，252 MB 资产）。

> **注意**：首次打开可能提示安装 `OutlineSmoothNormalsGenerator`，Package Manager 会从 Git URL 自动拉取。如失败，请手动添加：`https://github.com/AleFeng/OutlineSmoothNormalsGenerator.git?path=/Packages/com.alefeng.outlinesmoothnormalsgenerator`

---

## 📂 目录结构

```text
SumiToon/
├── Assets/
│   ├── ProductAssets/
│   │   ├── Material/               # 默认材质球
│   │   └── Prefab/Character/
│   │       ├── Kafka/              # 示例角色 (卡芙卡)
│   │       ├── Klee/               # 示例角色 (可莉)
│   │       └── Rossi/              # 实战角色 (洛茜)
│   │           ├── Materials/      # 自动生成的 20 个材质
│   │           ├── Textures/       # 18 张 D/N 贴图 + SDF 图集
│   │           └── *.fbx           # 主体 + 5 个细节网格
│   ├── Scenes/                     # DemoKafka / DemoKlee / DemoWater
│   ├── Settings/                   # URP 模板配置
│   └── URPSettings/                # 管线配置 + Volume
├── Packages/
│   └── com.sumitoon.urp/           # 核心 UPM 包
│       ├── Runtime/
│       │   ├── Shaders/            # Lit / WaterPlane / LitFaceSDF
│       │   ├── FaceSDFLightDriver.cs
│       │   └── MmdBlendShapeController.cs
│       ├── Editor/                 # 自定义 ShaderGUI
│       └── AlePostProcessingURP/   # 自定义后处理
└── ProjectSettings/
```

---

## 🛠️ 开发规范

本项目遵循严格的 Shader 代码规范，包括：
- **文件头**：必须包含用途 / 依赖 / 作者 / 许可。
- **命名前缀**：`_Tex` (贴图) / `_Color` (颜色) / `_Float` (浮点数) / `_Toggle` (开关) / `_Int` (整数)。
- **空间后缀**：`WS` (世界空间) / `OS` (对象空间) / `VS` (观察空间) / `CS` (裁剪空间) / `TS` (切线空间)。
- **魔法数字**：必须有注释说明其数学/物理意义。

---

## 📄 许可

- 本项目代码部分采用 [MIT License](./LICENSE)。
- 第三方组件许可见 [THIRD-PARTY-NOTICES.md](./THIRD-PARTY-NOTICES.md)。
- `Assets/ProductAssets/` 下的角色资产版权归原作者所有，**仅用于学习交流，不得商用**。
