# AleToonURP 代码审查报告

> 审查对象：AleToonURP v1.4.0（Lit + WaterPlane）
> 审查依据：《Shader代码规范.md》9 节 40+ 检查项
> 审查时间：2026-11-02
> 审查范围：8 个 Shader/HLSL 文件（不含 Editor C#）

---

## 总评

| 维度 | 得分 | 说明 |
|---|---|---|
| 文件头 | 0 / 10 | 全部缺失 |
| 命名规范 | 8 / 10 | 前缀体系完整，空间后缀齐全 |
| CBUFFER 分组 | 8 / 10 | 分组清晰，顺序对齐 Properties |
| 注释质量 | 5 / 10 | 密度高但重"是什么"轻"为什么" |
| 精度规范 | 7 / 10 | half/float 分工合理，个别位置待商榷 |
| 关键字管理 | 8 / 10 | 用 shader_feature_local + 枚举式互斥，很规范 |
| 格式 | 3 / 10 | Unicode 分区框、缩进不统一 |
| 文档配套 | 6 / 10 | 有 README/CHANGELOG，但无参数表 |
| **总分** | **45 / 80** | **及格偏上，可维护，但不符合商业交付标准** |

**一句话结论：** 这是一个"作者自己能维护、换人接手会痛苦"的项目。核心逻辑写得清楚，但缺少交接所需的元信息，且格式层面有跨平台风险。

---

## 一、文件头：0 / 10（全部缺失）

**检查结果：** 搜索 `copyright|author|作者|版权|@file|@brief`，**0 匹配**。

8 个文件全部没有文件头。举两个典型：

```hlsl
// LitMain.hlsl 第 1 行
//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 结构体定义 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
```

```hlsl
// Function.hlsl 第 1-2 行（空行 + 注释）
    //旋转UV uv:uv坐标 radia:弧度 pivot:锚点
```

**问题：**
1. 不知道文件用途（新人要读完才知道这是主片元逻辑）
2. 不知道依赖关系（哪些 hlsl 必须先 include）
3. 不知道作者和修改日期（出问题找谁）
4. 不知道许可（商用合规风险）

**建议（可直接复制）：**

```hlsl
// ============================================================
// 文件：LitMain.hlsl
// 用途：AleToonURP Lit 主片元逻辑（裁剪/基础色三色阶/高光/Rim/MatCap/自发光）
// 依赖：LitPropInput.hlsl（CBUFFER）、Function.hlsl（RotateUV）
// 作者：AleFeng    创建：-    最后修改：2026-11-02
// 许可：MIT
// ============================================================
```

**优先级：P0**（交接必查项）

---

## 二、命名规范：8 / 10

### ✅ 做得好的

**前缀体系完整且一致：**

| 类型 | 前缀 | 实例 |
|---|---|---|
| 贴图 | `_Tex` | `_TexRimLightMaskMap`、`_TexHighLightMap` |
| 颜色 | `_Color` | `_ColorShade1Color`、`_ColorRimLightColor` |
| 浮点 | `_Float` | `_FloatRimLightIntensity`、`_FloatOutlineWidth` |
| 开关 | `_Toggle` | `_ToggleRimLight`、`_ToggleShadowReceive` |
| 枚举 | `_Int` | `_IntClipType`、`_IntSurfaceType` |

**空间后缀齐全**（这是 Shader 最容易出 bug 的地方，作者做得很好）：

```hlsl
float3 viewDirWS;          // 世界空间
float3 normalDirWS;
float3 smoothNormalOS;     // 对象空间
half3 normalDirTS1;        // 切线空间
OUT.positionCS;            // 裁剪空间
```

**关键字全大写 + 下划线：** `_RIMLIGHT_ON`、`_BASEMAP_SHADE_THRESHOLDMAP_ON`

### ❌ 扣分项

**1. 命名与实际行为不符（作者自己标注了，但没有改）**

```hlsl
// WaterPlaneMain.hlsl 第 20-21 行
half _FloatWaveSpeedY; //GUI「速度Y轴」滑条 → 在 frag 中实际驱动 X/U 方向偏移（历史命名，为兼容既有材质不改变量名）
half _FloatWaveSpeedX; //GUI「速度X轴」滑条 → 在 frag 中实际驱动 Y/V 方向偏移（历史命名，为兼容既有材质不改变量名）
```

**问题：** X 变量驱动 Y、Y 变量驱动 X，名字和行为完全相反。虽然作者写了注释说明，但**这会让每个读代码的人多花 30 秒确认，且极易在改动时引入 bug**。

**建议：** 保留旧变量名（兼容材质），但在 shader 内**开头加别名**：
```hlsl
// 历史命名兼容：_FloatWaveSpeedY 实际驱动 U 方向
#define _WaveSpeedU _FloatWaveSpeedY
#define _WaveSpeedV _FloatWaveSpeedX
```
后面代码用 `_WaveSpeedU`，读代码不会再错。

**2. 命名长度不一致**

`Function.hlsl` 里 `hash22`、`perlin_noise`（下划线风格）和 `RotateUV`（驼峰）混用。虽是小问题，但团队规范应该统一。

---

## 三、CBUFFER 分组：8 / 10

### ✅ 做得好的

`LitPropInput.hlsl` 的 CBUFFER 分组清晰，**顺序与 Properties 块一致**，方便对照：

```hlsl
CBUFFER_START(UnityPerMaterial)
    //裁剪Clip
    float4 _TexClipMaskMap_ST;
    half _FloatClipIntensity;
    ...

    //基础贴图
    float4 _BaseMap_ST;
    half4 _BaseColor;
    ...
CBUFFER_END
```

### ⚠️ 待改进

**1. 没有"分组分隔线"统一格式**

有的分组用 `//裁剪Clip`，有的用 `// 基础贴图`，空格不统一。

**2. 缺少"这个参数属于哪个功能开关"的说明**

比如 `_FloatRimLightIntensity` 只在 `_RIMLIGHT_ON` 时生效，但 CBUFFER 里没标。

**建议：**
```hlsl
    // ---- 边缘光 RimLight（仅 _RIMLIGHT_ON 生效）----
    half4 _ColorRimLightColor;
    half _FloatRimLightIntensity;
```

---

## 四、注释质量：5 / 10

### ✅ 做得好的

**注释密度很高**，几乎每 2-3 行就有注释，比大多数商业项目强。

**关键位置有"为什么"：**

```hlsl
// WaterPlaneMain.hlsl 第 144 行
half fresnel = pow(saturate(1 - dot(normalWS, viewDirWS)),
                   max(5 - _FresnelFactor * 0.5, 1e-3));
// 指数下限：防 _FresnelFactor=10 时 pow(0,0) 出 NaN
```

**这行是全项目注释质量最高的地方**——解释了"为什么加 max"，而不是复述代码。

**非直觉写法有说明：**

```hlsl
// LitMain.hlsl 第 187 行
//暗部1 光照强度（用局部量，不写回 CBUFFER 成员）
float blur1 = _FloatBrightShade1Blur * 0.1;
```

### ❌ 主要问题

**1. 大量"复述代码"式注释（估计占 60%）**

```hlsl
//内置光照 方向
float3 builtInLightDir = float3(...);     // ← 注释 = 变量名直译

//基础贴图 采样
float4 colorBaseMap = SAMPLE_TEXTURE2D(...);  // ← 同上
```

**规范要求：** 注释写"为什么"，不写"是什么"。变量名已经说明"是什么"了。

**2. 魔法数字没有解释（10 处）**

```hlsl
realtimeLightColor *= _FloatRealtimeLightIntensity * 0.1;   // ← 为什么是 0.1？
float blur1 = _FloatBrightShade1Blur * 0.1;                 // ← 为什么是 0.1？
float blur2 = _FloatShade1Shade2Blur * 0.05;                // ← 为什么是 0.05？
uvMatCap = RotateUV(uvMatCap, _FloatMatCapRotate * 3.141592654, ...);  // ← π 应该用常量
```

**建议：**
```hlsl
// 光照强度基准：0.1 使默认参数下光照接近 1.0（美术手感基线）
realtimeLightColor *= _FloatRealtimeLightIntensity * 0.1;

// 羽化系数：0.1 把美术 0~10 的模糊滑块映射到 shader 需要的小数范围
float blur1 = _FloatBrightShade1Blur * 0.1;

#define PI 3.141592654  // 顶部定义，全文件复用
uvMatCap = RotateUV(uvMatCap, _FloatMatCapRotate * PI, ...);
```

**3. 注释与代码不符（1 处，潜在 bug 来源）**

`WaterPlaneMain.hlsl` 第 20-21 行那个"历史命名"问题，注释说"实际驱动 X"，但代码里用的是 `_FloatWaveSpeedY` 去乘 `_Time.x` 加到 UV 上。注释是对的，但**读者要读两遍才能确认**。

---

## 五、精度规范：7 / 10

**统计：** 154 个 `half` 声明，155 个 `float` 声明，比例接近 1:1。

### ✅ 做得好的

**位置/UV 用 float，颜色/光照用 half：**

```hlsl
float4 positionOS : POSITION;    // 位置 float ✓
float2 uv : TEXCOORD0;           // UV float ✓
half3 colorBaseMap;              // 颜色 half ✓
half shadowAttenuation;          // 光照 half ✓
```

### ⚠️ 待商榷

**1. 部分中间结果用了 `float`，但可以降精度**

```hlsl
float3 lightDirOnBaseMap = lightDirWS;    // 光照方向其实可以 half
float halfLambert = dot(...) * 0.5 + 0.5; // 片元内结果，half 够
```

移动端这些 `float` 会占双倍寄存器。**但要谨慎改**——有些 `float` 是因为精度测试发现 half 不够才升上去的，没实测不要盲目降。

**2. 没有 `real` 类型的使用**

URP 推荐用 `real`（自动按平台切 half/float）。项目全用显式 `half`/`float`，移动端和 PC 共用一份代码时不够灵活。

**建议：** 新代码优先用 `real`，老代码不动。

---

## 六、关键字管理：8 / 10

### ✅ 做得非常好（这部分是亮点）

**1. 材质级开关全部用 `shader_feature_local`：**

```hlsl
#pragma shader_feature_local _OUTLINE_ON
#pragma shader_feature_local _RIMLIGHT_ON
#pragma shader_feature_local _MATCAP_ON
```

**用 `shader_feature_local` 而不是 `multi_compile` 是对的**——材质级开关不需要全量编译，能大幅减少变体数。

**2. 互斥开关合并成枚举式声明（正确做法）：**

```hlsl
// 裁剪：三选一，不是 3 个独立 bool
#pragma shader_feature_local _CLIP_OFF _CLIP_DITHER _CLIP_ALPHA

// MatCap 混合：三选一
#pragma shader_feature_local _MATCAP_COLORBLEND_ADDITIVE _MATCAP_COLORBLEND_MULTIPLY _MATCAP_COLORBLEND_LERP

// 自发光：二选一
#pragma shader_feature_local _EMISSIVE_FIXED _EMISSIVE_ANIM
```

**这正好避免了变体爆炸**（3 个独立 bool = 8 组合，枚举式 = 3 变体）。作者这块做得很专业。

**3. URP 内置关键字用 `multi_compile`（正确）：**

```hlsl
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
#pragma multi_compile_fragment _ _SHADOWS_SOFT
```

### ⚠️ 待改进

**缺少变体数量预算说明。** 38 个 `#pragma` 声明，实际变体数是多少？没在文件里说明。建议在 `Lit.shader` 头部加：

```hlsl
// 变体预算：本项目 Lit.shader 编译后约 XXX 个变体
// 新增关键字前请评估：每个 shader_feature 增加 1~2 个变体
```

---

## 七、格式：3 / 10（问题最多）

### ❌ 严重问题

**1. Unicode 分区框（86 处）——跨编码会乱码，已实际发生**

```hlsl
//┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 结构体定义 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
//┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ 结构体定义 ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛
```

**实测证据：** 我第一次搬运时用 PowerShell `Get-Content -Encoding Default`（GBK）读 UTF-8 文件，这些框线字符全变成 `鈹忊攣鈹佲攣`。**这是真实发生过的损坏，不是理论风险。**

**建议：** 全部替换成 ASCII：
```hlsl
// ==================== 结构体定义 ====================
// ==================== 基础贴图 BaseMap ====================
```

**2. 缩进不统一**

同一文件里 4 空格、8 空格、12 空格混用：

```hlsl
    // 4 空格
        // 8 空格
            // 12 空格（条件编译块内）
```

**3. 行内注释空格不统一**

```hlsl
TEXTURE2D(_TexClipMaskMap); ... //裁剪贴图      ← 无空格
TEXTURE2D(_TexShadeThresholdMap); ... //暗部阈值贴图  ← 无空格
```

对比 `LitMain.hlsl` 里：
```hlsl
float2 uv0 = IN.uv; // uv0    ← 有空格
```

**规范要求：** 行内注释统一 `// 内容`（斜杠后一个空格）。

---

## 八、文档配套：6 / 10

### ✅ 有 README / CHANGELOG / 三语版本

### ❌ 缺参数说明表

README 有功能列表，但**没有"每个 Shader 属性干什么"的对照表**。美术接手时要逐个在 Inspector 里试。

**建议：** 在 README 加表格：

| 属性 | 类型 | 范围 | 说明 |
|---|---|---|---|
| `_FloatRimLightIntensity` | Float | 0~1 | 边缘光粗细，越大越细 |
| `_FloatRimLightInsideDistance` | Float | 0~1 | 边缘光起始深度，越大越贴边 |
| `_ColorRimLightColor` | Color | HDR | RGB=颜色，A=整体强度 |

---

## 九、按优先级的改进清单

### P0（交接前必须做）

| # | 项 | 工作量 |
|---|---|---|
| 1 | 8 个文件加文件头（用途/依赖/作者/许可） | 30 分钟 |
| 2 | Unicode 分区框换 ASCII | 20 分钟（批量替换） |
| 3 | 魔法数字加解释（10 处） | 20 分钟 |

### P1（团队协作前做）

| # | 项 | 工作量 |
|---|---|---|
| 4 | 缩进统一 4 空格 | 1 小时（用 IDE 格式化） |
| 5 | 行内注释统一空格 | 30 分钟 |
| 6 | `_FloatWaveSpeedX/Y` 加 `#define` 别名 | 10 分钟 |
| 7 | README 加参数说明表 | 1 小时 |

### P2（有时间再做）

| # | 项 | 工作量 |
|---|---|---|
| 8 | 复述式注释改"为什么"（挑核心 20 处） | 2 小时 |
| 9 | 变体数量统计并写进文件头 | 30 分钟 |
| 10 | 新代码用 `real` 类型 | 持续 |

---

## 附：值得学习的三个点

审查不只是挑错，这份代码有三处**做得比多数商业项目好**，值得你抄进自己的项目：

1. **枚举式关键字声明**（`_CLIP_OFF _CLIP_DITHER _CLIP_ALPHA`）——直接避免了变体爆炸，很多团队栽在这
2. **防 NaN 的 `max(x, 1e-3)` 保护 + 注释**（WaterPlaneMain.hlsl 第 144/158 行）——参数拉到极端不崩，且注释说明为什么
3. **空间后缀贯穿全项目**（`WS`/`OS`/`TS`/`CS`）——Shader bug 最大来源就是空间搞混，这个习惯能省大量调试时间

---

## 附：审查方法说明（你以后自己审项目可复用）

1. **先跑规范自检清单**（9 节 40 项），不靠感觉
2. **用 grep 统计客观数据**（文件头 0 匹配、Unicode 86 处、half/float 154/155）——数据比印象可靠
3. **每个问题给"证据 + 为什么 + 怎么改"**——只说"注释不好"没人会改
4. **分 P0/P1/P2**——商业项目里没人有时间为所有问题买单
5. **同时写"做得好"**——审查不是批斗，是让作者知道哪些要保持
