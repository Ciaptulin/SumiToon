# SumiToon

基于 [AleToonURP](https://github.com/AleFeng/AleToonURP) 的 Unity URP NPR（二次元卡通）渲染学习项目。

> 本项目是**学习性质**的工程，不是商业产品。目的是通过阅读、手敲、改造一个成熟的 NPR 管线，理解卡通渲染的完整实现，并沉淀自己的工程规范。

---

## 项目性质

| 层 | 内容 | 归属 |
|---|---|---|
| `Packages/com.sumitoon.urp/` | AleToonURP 原版代码 + 规范化修改（UPM 包） | 基于 AleFeng 的 MIT 项目 |
| `Learning/` | 手敲记录、学习笔记、代码审查报告 | 本人 |
| `Assets/ProductAssets/` | 角色模型与场景（学习用，不商用） | miHoYo / 第三方 |
| `Assets/URPSettings/` | 管线与 Volume 配置 | 基于 AleToonURP 配置 |

**第三方组件与许可**详见 [THIRD-PARTY-NOTICES.md](./THIRD-PARTY-NOTICES.md)。

---

## 📚 文档索引

| 文档 | 面向 | 内容 |
|---|---|---|
| [docs/GETTING_STARTED.md](./docs/GETTING_STARTED.md) | **新手** | 10 分钟从 clone 到看到 Demo 效果，含验证清单 |
| [docs/ARCHITECTURE.md](./docs/ARCHITECTURE.md) | 开发者 | 目录职责、Shader 文件关系、代码阅读路线、数据流 |
| [docs/TROUBLESHOOTING.md](./docs/TROUBLESHOOTING.md) | 遇到问题的人 | 按现象查找：材质粉红、场景黑、描边消失等 |
| [docs/CONTRIBUTING.md](./docs/CONTRIBUTING.md) | 贡献者 | 提交规范、代码规范、变体管理、Git 流程 |
| [Document~/UserManual.md](./Document~/UserManual.md) | 美术 / TA | 每个 Shader 参数的详细说明（含截图） |
| [THIRD-PARTY-NOTICES.md](./THIRD-PARTY-NOTICES.md) | 合规 | 第三方组件来源、版本、许可 |
| [CHANGELOG.md](./CHANGELOG.md) | 所有人 | 版本变更记录 |

**第一次接触这个项目？从 [docs/GETTING_STARTED.md](./docs/GETTING_STARTED.md) 开始。**

---

## 环境要求

- **Unity**：2023.2.20f1
- **URP**：16.0.6
- **Git LFS**：必须安装（资产走 LFS）

---

## 安装

```bash
git clone <repo-url>
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
├── Learning/                      # 学习记录（不进 Unity 编译）
│   ├── handwritten/               # 手敲的 4 个核心 HLSL
│   └── notes/                     # 学习笔记与代码审查报告
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
