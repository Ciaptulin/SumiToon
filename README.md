D:\Code\UnityProject\SumiToon\README.md 读完了，共 108 行

1: # SumiToon
2: 
3: **SumiToon** 是一个面向 Unity URP (Universal Render Pipeline) 的工业级 NPR（二次元卡通）渲染管线与 Shader 解决方案。
4: 
5: 本项目最初起源于 AleToonURP（MIT 协议），但在本仓库中，已完成了**从基础研究 Demo 到商业化工程包的彻底重构**。它解决了原版中存在的大量底层隐患，包括 URP 16 的 API 迁移、内存泄漏修复以及资产序列化损坏等深水区问题。
6: 
7: ## ✨ 为什么使用 SumiToon？
8: 
9: - **彻底修复底层内存泄漏**：重构了后处理管线的 RT 释放逻辑，避免了长时间运行的显存溢出。
10: - **现代化 UPM 工程规范**：摒弃了散装脚本，全量迁移为规范的 UPM 包（`com.sumitoon.urp`），开箱即用。
11: - **无缝升级 URP 16**：彻底移除了 `RenderTargetHandle` 等废弃 API，全面拥抱 `RTHandle` 体系，兼容 Unity 2023.2+。
12: - **抢救损坏的后处理资产**：修复了上游版本遗留的 Volume 资产序列化丢失与字段不匹配问题。
13: - **工业化 CI 与测试**：内置 GitHub Actions 自动化校验与 EditMode 测试，保证代码规范与 Shader 内存布局一致性。
14: 
15: > 🔗 **[点击查看详细的架构重构与排雷贡献记录 (CONTRIBUTION.md)](./CONTRIBUTION.md)**

---

## 🚀 实战案例：洛茜 (Rossi) 角色全流程接入与管线扩展

本项目不仅是一个 Shader 包，更经过了重度二次元角色（《明日方舟：终末地》洛茜）的工业化落地验证。在此过程中，针对实际美术需求对管线进行了深度扩展：

### 1. 面部 SDF 阴影系统 (FaceSDF) 跨引擎重构
- **跨引擎数据迁移**：将 Blender 中复杂的 3x3 图集面部阴影逻辑（9帧差分 + 阈值图）完整迁移至 Unity URP。
- **解决左右不对称与死黑**：重写 `LitFaceSDF.hlsl`，通过本地空间光照方向动态选择正/负图集，结合平滑阈值，彻底解决跨引擎带来的 UV 翻转错位、背光死黑、左右脸阴影不对称等问题。
- **光照解耦驱动**：新增 `FaceSDFLightDriver.cs`，实时将主光方向转换至模型本地空间，确保角色旋转时面部阴影依然精准跟随主光。

### 2. Shader 与 GUI 扩展
- **新增属性与面板**：在 `Lit.shader` 中增加 SDF 阴影专属属性，并修改 `ShaderGUILit.cs` 实现自定义折叠面板，支持调试帧与阴影阈值实时预览。
- **MMD 骨骼与表情适配**：新增 `MmdBlendShapeController.cs`，完美驱动 41 个日文命名 BlendShape（眨眼、口型），并支持自动呼吸/眨眼循环。

### 3. 资产管线工业化落地
- **FBX 导出优化**：剥离冗余几何节点，修复金属重叠面，完成 514 骨 MMD 骨架与 5 套细节网格的精确蒙皮导出。
- **材质自动化装配**：编写 Editor 工具，实现 20 个材质的自动生成、贴图绑定、透明裁剪（Dither）及关键词一键配置。
- **渲染管线清理**：移除 Volume 中冗余的测试后处理，建立标准的 Tonemapping + Bloom + 颜色校正 二次元渲染流程。
16: 
17: ---
18: 
19: ## 📦 安装方式
20: 
21: 在 Unity 中打开 `Window → Package Manager`，点击 `+ → Add package from git URL...`，输入：
22: 
23: ```
24: https://github.com/Ciaptulin/SumiToon.git?path=/Packages/com.sumitoon.urp
25: ```
26: 
27: ---
28: 
29: ## 环境要求
30: 
31: - **Unity**：2023.2.20f1
32: - **URP**：16.0.6
33: - **Git LFS**：必须安装（资产走 LFS）
34: 
35: ---
36: 
37: ## 安装
38: 
39: ```bash
40: git clone https://github.com/Ciaptulin/SumiToon.git
41: cd SumiToon
42: git lfs install
43: git lfs pull
44: ```
45: 
46: 用 Unity 2023.2.20f1 打开项目，等待首次导入完成（约 3~5 分钟，252 MB 资产）。
47: 
48: **首次打开可能提示安装 `OutlineSmoothNormalsGenerator`**，Package Manager 会从 Git URL 自动拉取。如失败，手动添加：
49: 
50: ```
51: https://github.com/AleFeng/OutlineSmoothNormalsGenerator.git?path=/Packages/com.alefeng.outlinesmoothnormalsgenerator
52: ```
53: 
54: ---
55: 
56: ## 目录结构
57: 
58: ```
59: SumiToon/
60: ├── Assets/
61: │   ├── PluginsDeveloper/
62: │   │   ├── Runtime/Shaders/       # Lit / WaterPlane Shader + HLSL
63: │   │   ├── Editor/                # 自定义 ShaderGUI
64: │   │   ├── Example/               # 示例材质与贴图
65: │   │   ├── AleCameraRover/        # 相机漫游组件
66: │   │   └── AlePostProcessingURP/  # 自定义后处理
67: │   ├── ProductAssets/             # 角色 / 场景模型与材质
68: │   ├── Scenes/                    # DemoKafka / DemoKlee / DemoWater
69: │   ├── URPSettings/               # 管线配置 + Volume
70: │   └── Settings/                  # URP 模板配置（保留）
71: ├── Packages/                      # manifest.json
72: ├── ProjectSettings/
73: ├── LICENSE
74: ├── THIRD-PARTY-NOTICES.md
75: ├── CHANGELOG.md
76: ├── .gitignore
77: ├── .gitattributes
78: └── .editorconfig
79: ```
80: 
81: ---
82: 
83: ## 主要特性（继承自 AleToonURP）
84: 
85: - **NPR 卡通渲染 Lit Shader**：三色阶明暗、外描边、边缘光、高光、自发光、MatCap
86: - **NPR 水面**：深度水色、多层波形、反射折射、边缘泡沫
87: - **UberShader**：通过宏开关生成变体，按需启用效果
88: - **自定义 ShaderGUI**：折叠式材质面板
89: - **SRP Batcher 兼容**：CBUFFER 按 UnityPerMaterial 组织
90: 
91: ---
92: 
93: ## 开发规范
94: 
95: 本项目遵循严格的 Shader 代码规范，包括：
96: - 文件头（用途 / 依赖 / 作者 / 许可）
97: - 命名前缀（`_Tex` / `_Color` / `_Float` / `_Toggle` / `_Int`）
98: - 空间后缀（`WS` / `OS` / `VS` / `CS` / `TS`）
99: - 魔法数字必须有注释说明
100: 
101: ---
102: 
103: ## 许可
104: 
105: 本项目代码部分采用 [MIT License](./LICENSE)。
106: 第三方组件许可见 [THIRD-PARTY-NOTICES.md](./THIRD-PARTY-NOTICES.md)。
107: `Assets/ProductAssets/` 下的角色资产版权归 miHoYo，**仅用于学习，不得商用**。
108: 

(全文 108 行都在这儿了)