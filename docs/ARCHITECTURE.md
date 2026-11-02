# 架构与代码阅读指南（ARCHITECTURE）

> 目标：让新接手的人能在 **1 小时内**建立对项目的心智模型，知道"哪个文件负责什么"。

---

## 1. 目录职责

\`\`\`
SumiToon/
├── Assets/
│   ├── PluginsDeveloper/              # 核心代码（基于 AleToonURP）
│   │   ├── Runtime/Shaders/           # Shader 与 HLSL
│   │   ├── Editor/                    # 自定义 ShaderGUI（材质面板）
│   │   ├── Example/                   # 示例材质 + 贴图
│   │   ├── AleCameraRover/            # 相机漫游组件（演示用）
│   │   └── AlePostProcessingURP/      # 自定义后处理（13 种效果）
│   ├── ProductAssets/                 # 角色模型 / 场景（学习用，不商用）
│   ├── Scenes/                        # Demo 场景
│   ├── URPSettings/                   # 管线配置 + Volume
│   └── Settings/                      # URP 模板配置（保留，供对照）
│
│
├── Document~/                         # 用户手册（~ 后缀 → Unity 不导入）
│   ├── UserManual.md                  # 每个 Shader 参数说明
│   └── Image/                         # 手册截图
│
├── docs/                              # 工程文档
│   ├── GETTING_STARTED.md
│   ├── ARCHITECTURE.md                # 本文件
│   ├── TROUBLESHOOTING.md
│   └── CONTRIBUTING.md
│
├── Packages/                          # manifest.json（依赖）
├── ProjectSettings/                   # 项目设置
└── README.md                          # 入口
\`\`\`

**规则：**
- \`~\` 后缀的目录（如 \`Document~\`）Unity **不会导入**，适合放文档和图片
- \`Learning/\` 在 \`Assets\` 外，Unity 不会编译，可以放心放 \`.hlsl\` 学习副本
- \`Assets/Settings/\` 是 URP 模板默认配置，**保留但不使用**，删掉也行

---

## 2. Shader 文件关系（Lit）

这是项目最核心的部分。**读代码从这里开始**。

\`\`\`
Lit.shader                       ← 入口：Properties + 4 个 Pass + 关键字 pragma
├── Pass 1: ForwardLit
│   ├── #include LitPropInput.hlsl   ← CBUFFER + 贴图声明 + 输入结构
│   ├── #include Function.hlsl       ← 工具函数（RotateUV / hash / 噪声）
│   └── #include LitMain.hlsl        ← 主片元逻辑（核心）
│
├── Pass 2: Outline
│   ├── #include LitPropInput.hlsl
│   ├── #include Function.hlsl
│   ├── #include OutlineSmoothNormals.hlsl  ← 外部包（平滑法线解码 + 外扩）
│   └── #include LitOutline.hlsl           ← 描边逻辑
│
├── Pass 3: ShadowCaster
│   ├── #include LitPropInput.hlsl
│   └── #include LitShadowDepth.hlsl       ← 阴影投射（裁剪轮廓一致）
│
└── Pass 4: DepthOnly
    ├── #include LitPropInput.hlsl
    └── #include LitShadowDepth.hlsl
\`\`\`

### 每个文件干什么

| 文件 | 职责 | 读的时候关注 |
|---|---|---|
| **Lit.shader** | 入口：属性面板、Pass 组织、关键字声明 | Properties 块、\`#pragma\` 变体、Pass 的 Cull/ZWrite/Blend |
| **LitPropInput.hlsl** | CBUFFER + 所有贴图声明 + 输入结构 | \`CBUFFER_START(UnityPerMaterial)\` 里的参数分组 |
| **Function.hlsl** | 工具函数：RotateUV、hash22、perlin_noise | 和具体效果无关，可独立看 |
| **LitMain.hlsl** | **主片元逻辑**（核心） | 见下面的"阅读路线" |
| **LitOutline.hlsl** | 描边 Pass 的顶点+片元 | 平滑法线解码、\`OSN_ApplyOutlineOffset\` |
| **LitShadowDepth.hlsl** | 阴影/深度 Pass | 为什么不用 URP 自带 \`_ALPHATEST_ON\` |

---

## 3. LitMain.hlsl 阅读路线（按顺序读）

\`LitMain.hlsl\` 约 400 行，分 9 个块。**按这个顺序读，每块只看"输入→输出"**：

| 顺序 | 行号 | 块 | 作用 | 关键变量 |
|---|---|---|---|---|
| 1 | 8-27 | 结构体 | \`AttributesAleToon\` / \`VaryingsAleToon\` | 语义（POSITION / NORMAL / TEXCOORD） |
| 2 | 33-70 | vert | 顶点着色：坐标转换、法线/切线传世界空间 | \`positionWS\` / \`normalWS\` / \`tangentWS\` |
| 3 | 78-98 | 基础贴图 + 裁剪 | 采样 Albedo，\`clip()\` 做挖孔/透明 | \`colorBaseMap\` / \`clipValue\` |
| 4 | 101-107 | 法线贴图 | 采样法线，TBN 转世界空间 | \`normalDirTex\` / \`binormalWS\` |
| 5 | 109-131 | 光照 | 环境光（SH）+ 主光 + 附加光 + 阴影 | \`envLightColor\` / \`realtimeLightColor\` / \`shadowAttenuation\` |
| 6 | 146-196 | **三色阶** | N·L → 阈值 → 三色 lerp | \`halfLambert\` / \`lightIntensityShade1/2\` |
| 7 | 199-222 | 高光 | 半程向量 + 色阶/柔边 + 遮罩 | \`highLightFactor\` |
| 8 | 225-278 | **边缘光** | N·V + pow + 暗部遮罩 + 遮罩贴图 | \`rimLightFactor\` / \`shadeMaskFactor\` |
| 9 | 281-312 | MatCap | 观察空间法线 XY 当 UV 查表 | \`uvMatCap\` |
| 10 | 315-374 | 自发光 | 固定/动画（UV 滚动 + 颜色变化） | \`colorEmissiveFinal\` |
| 11 | 377-398 | 输出 | Alpha 处理 + 雾效 | \`colorFinal\` |

**每个效果都是独立的 \`#if defined(_XXX_ON)\` 块**，可以单独读、单独开关，互不干扰。

---

## 4. 数据流（一张图理解）

\`\`\`
顶点输入
  ↓ vert
  ├── positionWS（世界位置）
  ├── normalWS（世界法线）
  ├── tangentWS（世界切线 + 手性符号）
  ├── uv（原始 UV，不做 TRANSFORM_TEX）
  └── fogFactor
  ↓ 插值
片元输入
  ↓
  ├── 基础贴图 → colorBaseMap → 三色阶 → colorFinalBlend
  ├── 法线贴图 → normalDirTex → TBN → 各效果用
  ├── 光照 → envLightColor + realtimeLightColor → colorLightBlend
  ├── 高光 → colorHighLight（加到 colorFinalBlend）
  ├── 边缘光 → colorRimLight（加到 colorFinalBlend）
  ├── MatCap → colorMatCap（混合到 colorFinalBlend）
  ├── 自发光 → colorEmissive（加到 colorFinalBlend）
  ↓
  MixFog → 最终颜色
\`\`\`

**核心认知：** 所有效果都在 \`colorFinalBlend\` 上做**加法或混合**，没有覆盖。所以效果可以任意组合，顺序影响不大（除了三色阶必须在最前面定基础色）。

---

## 5. WaterPlane 阅读路线

\`WaterPlaneMain.hlsl\` 约 170 行，7 个块：

| 顺序 | 行号 | 块 | 作用 |
|---|---|---|---|
| 1 | 93-101 | UV 动画 | 两层法线交叉采样，UV 随时间移动 |
| 2 | 103-113 | 法线采样 | 两张法线叠加 + TBN 转世界空间 |
| 3 | 115-119 | 深度采样 | \`SampleSceneDepth\` → 线性深度 → \`depthDis\` |
| 4 | 121-126 | 水颜色 | \`lerp(浅水色, 深水色, depthDis)\` |
| 5 | 128-136 | 折射 | \`SampleSceneColor\` + 屏幕偏移 |
| 6 | 138-149 | 反射 | \`reflect()\` + CubeMap + 菲涅尔 |
| 7 | 151-163 | 边缘泡沫 | 深度差阈值 + \`smoothstep\` |

**关键概念：\`depthDis = 场景深度 - 水面自身深度\`**，差越大 = 水越深 = 颜色越接近深水色。

---

## 6. 关键字与变体（读 Lit.shader 时注意）

\`Lit.shader\` 里有两类关键字：

| 类型 | 用途 | 例子 |
|---|---|---|
| \`shader_feature_local\` | **材质级开关**，只编译用到的变体 | \`_OUTLINE_ON\` / \`_RIMLIGHT_ON\` / \`_MATCAP_ON\` |
| \`multi_compile\` | **运行时切换**，全量编译 | \`_MAIN_LIGHT_SHADOWS\` / \`_ADDITIONAL_LIGHTS\` |

**互斥的开关用枚举式声明**（避免变体爆炸）：

\`\`\`hlsl
// 正确：三选一，只有 3 个变体
#pragma shader_feature_local _CLIP_OFF _CLIP_DITHER _CLIP_ALPHA

// 错误：3 个独立 bool，理论 8 个组合
#pragma shader_feature_local _CLIP_OFF
#pragma shader_feature_local _CLIP_DITHER
#pragma shader_feature_local _CLIP_ALPHA
\`\`\`

---

## 7. 代码阅读的 3 个建议

1. **先跑起来，再读代码**——看到效果才知道每行在干什么。按 [GETTING_STARTED.md](./GETTING_STARTED.md) 先跑通。
2. **从 LitMain.hlsl 的边缘光（225-278 行）开始读**——它是最独立、最容易理解的效果块，覆盖了"采样 → 计算 → 混合"的完整流程。
3. **每读一个效果，就去材质面板上关掉对应的开关**——对比看效果消失，理解这个效果的作用。

---

## 8. 常见困惑

**Q：为什么 \`OUT.uv = IN.texcoord\` 不用 \`TRANSFORM_TEX\`？**
A：每个贴图有自己的 \`_ST\`（Tiling/Offset），在采样时用 \`TRANSFORM_TEX(uv0, _TexXXXMap)\` 单独处理，不在顶点统一处理。

**Q：为什么阴影坐标不在顶点传，在片元重算？**
A：见 \`LitMain.hlsl\` 第 61 行注释——逐像素重算 \`TransformWorldToShadowCoord(positionWS)\` 更精确，避免插值误差。

**Q：\`Cull Front\` 的描边 Pass 为什么不会覆盖正面？**
A：深度测试。外扩的背面在轮廓内被正面模型挡住（更近），只有轮廓外没东西遮挡的部分露出来形成描边。

**Q：为什么法线贴图不能勾 sRGB？**
A：法线是**数据**不是颜色。勾了 sRGB，GPU 会做非线性解码，把法线方向掰歪，光照就错了。
