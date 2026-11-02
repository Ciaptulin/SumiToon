# SumiToon

**SumiToon** 是一个面向 Unity URP (Universal Render Pipeline) 的工业级 NPR（二次元卡通）渲染管线与 Shader 解决方案。

本项目最初起源于 AleToonURP（MIT 协议），但在本仓库中，已完成了**从基础研究 Demo 到商业化工程包的彻底重构**。它解决了原版中存在的大量底层隐患，包括 URP 16 的 API 迁移、内存泄漏修复以及资产序列化损坏等深水区问题。

## ✨ 为什么使用 SumiToon？

- **彻底修复底层内存泄漏**：重构了后处理管线的 RT 释放逻辑，避免了长时间运行的显存溢出。
- **现代化 UPM 工程规范**：摒弃了散装脚本，全量迁移为规范的 UPM 包（`com.sumitoon.urp`），开箱即用。
- **无缝升级 URP 16**：彻底移除了 `RenderTargetHandle` 等废弃 API，全面拥抱 `RTHandle` 体系，兼容 Unity 2023.2+。
- **抢救损坏的后处理资产**：修复了上游版本遗留的 Volume 资产序列化丢失与字段不匹配问题。
- **工业化 CI 与测试**：内置 GitHub Actions 自动化校验与 EditMode 测试，保证代码规范与 Shader 内存布局一致性。

> 🔗 **[点击查看详细的架构重构与排雷贡献记录 (CONTRIBUTION.md)](./CONTRIBUTION.md)**

---

## 📦 安装方式

在 Unity 中打开 `Window → Package Manager`，点击 `+ → Add package from git URL...`，输入：

```
https://github.com/Ciaptulin/SumiToon.git?path=/Packages/com.sumitoon.urp
```

---

## 环境要求

- **Unity**：2023.2.20f1
- **URP**：16.0.6
- **Git LFS**：必须安装（资产走 LFS）

---

## 安装

```bash
git clone https://github.com/Ciaptulin/SumiToon.git
cd SumiToon
git lfs install
git lfs pull
```

用 Unity 2023.2.20f1 打开项目，等待首次导入完成（约 3~5 分钟，252 MB 资产）。

**首次打开可能提示安装 `OutlineSmoothNormalsGenerator`**，Package Manager 会从 Git URL 自动拉取。如失败，手动添加：

```
https://github.com/AleFeng/OutlineSmoothNormalsGenerator.git?path=/Packages/com.alefeng.outlinesmoothnormalsgenerator
```

---

## 目录结构

```
SumiToon/
├── Assets/
│   ├── PluginsDeveloper/
│   │   ├── Runtime/Shaders/       # Lit / WaterPlane Shader + HLSL
│   │   ├── Editor/                # 自定义 ShaderGUI
│   │   ├── Example/               # 示例材质与贴图
│   │   ├── AleCameraRover/        # 相机漫游组件
│   │   └── AlePostProcessingURP/  # 自定义后处理
│   ├── ProductAssets/             # 角色 / 场景模型与材质
│   ├── Scenes/                    # DemoKafka / DemoKlee / DemoWater
│   ├── URPSettings/               # 管线配置 + Volume
│   └── Settings/                  # URP 模板配置（保留）
├── Packages/                      # manifest.json
├── ProjectSettings/
├── LICENSE
├── THIRD-PARTY-NOTICES.md
├── CHANGELOG.md
├── .gitignore
├── .gitattributes
└── .editorconfig
```

---

## 主要特性（继承自 AleToonURP）

- **NPR 卡通渲染 Lit Shader**：三色阶明暗、外描边、边缘光、高光、自发光、MatCap
- **NPR 水面**：深度水色、多层波形、反射折射、边缘泡沫
- **UberShader**：通过宏开关生成变体，按需启用效果
- **自定义 ShaderGUI**：折叠式材质面板
- **SRP Batcher 兼容**：CBUFFER 按 UnityPerMaterial 组织

---

## 开发规范

本项目遵循 [Shader 代码规范](./Learning/notes/)（如有），包括：
- 文件头（用途 / 依赖 / 作者 / 许可）
- 命名前缀（`_Tex` / `_Color` / `_Float` / `_Toggle` / `_Int`）
- 空间后缀（`WS` / `OS` / `VS` / `CS` / `TS`）
- 魔法数字必须有注释说明

---

## 许可

本项目代码部分采用 [MIT License](./LICENSE)。
第三方组件许可见 [THIRD-PARTY-NOTICES.md](./THIRD-PARTY-NOTICES.md)。
`Assets/ProductAssets/` 下的角色资产版权归 miHoYo，**仅用于学习，不得商用**。
