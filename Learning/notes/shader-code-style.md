# Unity URP Shader 代码规范（可执行检查版）

> 用途：新项目起步、Code Review 对照、个人自检
> 原则：每条都可判定"是/否"，不写"要清晰"这种无法检查的话

---

## 一、文件头（每个 .shader / .hlsl 必须有）

```hlsl
// ============================================================
// 文件：LitMain.hlsl
// 用途：AleToonURP Lit Shader 的主片元逻辑（基础色/高光/Rim/MatCap/自发光）
// 依赖：LitPropInput.hlsl（CBUFFER）、Function.hlsl（工具函数）
// 作者：xxx    创建：2026-11-02    最后修改：2026-11-02
// 许可：MIT（或项目内部许可）
// ============================================================
```

**检查项：**
- [ ] 有文件名
- [ ] 有一句话用途（不是"这是一个shader"）
- [ ] 有依赖说明（include 了哪些本地文件）
- [ ] 有作者/日期/许可

**为什么：** 商业项目交接时，第一眼看文件头就知道这文件干什么、能不能删、找谁问。

---

## 二、命名规范

| 类型 | 规则 | 正例 | 反例 |
|---|---|---|---|
| Shader 属性-贴图 | `_Tex` 前缀 | `_TexRimLightMaskMap` | `_RimMask` |
| Shader 属性-颜色 | `_Color` 前缀 | `_ColorShade1Color` | `_ShadeColor1` |
| Shader 属性-浮点 | `_Float` 前缀 | `_FloatRimLightIntensity` | `_RimIntensity` |
| Shader 属性-开关 | `_Toggle` 前缀 | `_ToggleRimLight` | `_RimOn` |
| Shader 属性-枚举 | `_Int` 前缀 | `_IntClipType` | `_ClipType` |
| 关键字 | `_全大写下划线` | `_RIMLIGHT_ON` | `RIM_ON` |
| 全局变量 | 小驼峰 | `viewDirWS` | `ViewDir` |
| 函数 | 大驼峰 | `RotateUV` | `rotate_uv` |
| 常量/宏 | 全大写 | `MAX_LIGHT_COUNT` | `maxLight` |

**后缀约定（强烈建议）：**
- `WS` = 世界空间，`OS` = 对象空间，`VS` = 观察空间，`CS` = 裁剪空间，`TS` = 切线空间
- 正例：`normalWS`、`positionOS`、`normalDirTS`
- 反例：`normal`、`pos`（看不出在哪个空间，是 Shader 最常见的 bug 来源）

**检查项：**
- [ ] 属性有前缀
- [ ] 关键字全大写
- [ ] 空间后缀齐全（这是 Shader 特有的，必须查）

---

## 三、CBUFFER 分组

```hlsl
CBUFFER_START(UnityPerMaterial)
    // ---- 裁剪 Clip ----
    float4 _TexClipMaskMap_ST;
    half _FloatClipIntensity;

    // ---- 基础贴图 BaseMap ----
    float4 _BaseMap_ST;
    half4 _BaseColor;
CBUFFER_END
```

**检查项：**
- [ ] 所有材质属性都在 `CBUFFER_START(UnityPerMaterial)` 内
- [ ] 按功能分组，每组有注释头
- [ ] 组内顺序与 Properties 块顺序一致（方便对照）
- [ ] **没有把 `half` 和 `float` 随意混用**（见第五节）

**为什么：** SRP Batcher 要求 CBUFFER 布局一致；分组乱会导致"想加一个参数不知道插哪"，改错位置可能破坏对齐。

---

## 四、注释规范

### 4.1 行内注释写"为什么"，不写"是什么"

```hlsl
// 反例：注释重复代码
float3 builtInLightDir = float3(...);  // 内置光照方向

// 正例：注释解释为什么
// 内置光照方向：美术可手动指定光照角度，用于固定镜头演出
float3 builtInLightDir = float3(...);
```

### 4.2 关键设计必须有"为什么"

凡是以下情况，**必须**有注释解释为什么这么写：
- 魔法数字（`* 0.5 + 0.5`、`* 0.1`、`exp2(lerp(3,0,x))`）
- 防 NaN / 防除零的保护（`max(x, 1e-4)`）
- 非直觉的写法（`min(x, 0)` 当门用）
- 平台/版本兼容的 hack

```hlsl
// 指数下限 1e-3：防止 _FresnelFactor=10 时 pow(0,0) 出 NaN
pow(saturate(1 - NdotV), max(5 - _FresnelFactor * 0.5, 1e-3))
```

### 4.3 TODO / FIXME / HACK 标记

```hlsl
// TODO(姓名, 2026-11-02): 移动端半精度下 rim 会溢出，待验证
// FIXME(姓名, 2026-11-02): 这里假设了光源方向归一化，外部传入未归一化会错
// HACK(姓名, 2026-11-02): 绕过 URP 14 的 SampleSceneColor 兼容问题
```

**检查项：**
- [ ] 没有"注释复述代码"
- [ ] 所有魔法数字有解释
- [ ] 所有防 NaN/防除零有解释
- [ ] TODO/FIXME 带姓名和日期（否则永远没人处理）

---

## 五、精度规范

| 数据类型 | 用途 | 例子 |
|---|---|---|
| `float` | 位置、UV、世界坐标、矩阵 | `positionWS`、`uv0` |
| `half` | 颜色、光照强度、法线（移动端） | `colorBaseMap`、`NdotL` |
| `real` | URP 推荐：按平台自动切 half/float | 新代码优先用 |

**检查项：**
- [ ] 位置/UV 不用 `half`（精度不够会抖动）
- [ ] 颜色/光照不用 `float`（浪费带宽）
- [ ] 同一个变量不要一半 `half` 一半 `float` 混算（隐式转换有开销）

---

## 六、关键字与变体

**检查项：**
- [ ] 每个 `#pragma multi_compile` / `shader_feature` 有注释说明用途
- [ ] 材质级开关用 `shader_feature`，运行时切换用 `multi_compile`
- [ ] 互斥的开关合并成枚举（避免 2^N 爆炸）
- [ ] 有变体数量预算（比如"本 shader 不超过 200 个变体"）

---

## 七、格式

**检查项：**
- [ ] 缩进统一（4 空格 或 Tab，团队二选一）
- [ ] 大括号不换行（`if (x) {`）或换行，团队统一
- [ ] 分区框用 ASCII（`// ===== 分区 =====`），**不用 Unicode 框线**（跨编码会乱码，你已经踩过这个坑）
- [ ] 文件末尾有空行
- [ ] 没有行尾多余空格

---

## 八、文档配套

**检查项：**
- [ ] `README` 里有参数说明表（每个对外属性干什么）
- [ ] `CHANGELOG` 按版本记录改动
- [ ] 复杂效果有配图或 GIF

---

## 九、自检流程（提交前跑一遍）

1. 文件头齐全？
2. 命名前缀 + 空间后缀齐全？
3. CBUFFER 分组且顺序对齐？
4. 魔法数字都有解释？
5. 防 NaN 保护都有注释？
6. 精度符合规范？
7. 分区框是 ASCII？
8. CHANGELOG 更新了？

---

## 附：本规范的来源

- Unity 官方 Shader 编写建议（CBUFFER、精度、变体）
- 商业项目 Code Review 常见退回项
- AleToonURP 实际代码的正反例对照（见 review 报告）
