# 常见问题（TROUBLESHOOTING）

> 按"现象"查找，先做**快速诊断**，再看**根因和修复**。

---

## 快速诊断表

| 现象 | 最可能原因 | 跳到 |
|---|---|---|
| 材质全粉红 / 紫红 | Shader 编译失败 | [1](#1-材质全粉红) |
| 场景全黑 | 渲染管线没指向正确资产 | [2](#2-场景全黑) |
| 模型是白模 / 灰色 | 材质引用断裂 | [3](#3-模型白模) |
| 描边没了 / 描边错乱 | 平滑法线包没装 / 法线与烘焙不一致 | [4](#4-描边问题) |
| 后处理效果消失 | AlePostProcessingURP 编译失败 | [5](#5-后处理失效) |
| 中文注释乱码 | 文件编码不是 UTF-8 | [6](#6-中文乱码) |
| 拉取项目后资产是占位符 | LFS 没拉 | [7](#7-资产占位符) |
| Console 报 \`OSN_GetSmoothNormalOS\` 未定义 | OutlineSmoothNormalsGenerator 没装 | [4](#4-描边问题) |

---

## 1. 材质全粉红

**现象：** 场景里所有角色/场景都是粉红或紫红色。

**快速诊断：**
1. 打开 \`Window → General → Console\`
2. 看红色报错，搜 \`Shader error\` 或 \`undeclared identifier\`

**常见原因和修复：**

| 报错关键词 | 原因 | 修复 |
|---|---|---|
| \`OSN_GetSmoothNormalOS\` / \`OSN_ApplyOutlineOffset\` 未定义 | 没装 OutlineSmoothNormalsGenerator | 见 [4](#4-描边问题) |
| \`_FloatXXX\` 未定义 | LitPropInput.hlsl 缺失或 CBUFFER 不完整 | \`git status\` 检查文件完整性 |
| \`redefinition of 'PI'\` | 自己加了 \`#define PI\` 和 URP 冲突 | 删掉自定义 PI，用 URP 内置的 |
| 无报错但全粉 | 管线资产没设置 | 见 [2](#2-场景全黑) |

---

## 2. 场景全黑

**现象：** 打开场景后一片黑，或只有 UI 能看见。

**快速诊断：**
\`Project Settings → Graphics → Scriptable Render Pipeline Settings\`

**修复：**
1. 如果为空：选 \`Assets/URPSettings/AleToonUPR_Settings\`
2. 如果指向 \`Assets/Settings/URP-HighFidelity\` 等模板：改成 AleToon 的
3. 改完 Unity 会重新编译 Shader，等 1~2 分钟

**验证：** \`Project Settings → Quality\` 里当前档位的 \`Render Pipeline Asset\` 也应该是 AleToon 的。

---

## 3. 模型白模

**现象：** 角色是白色/灰色，没有贴图。

**快速诊断：**
1. 在 Project 里找到 \`Assets/ProductAssets/Prefab/Character/Kafka/Materials/\`
2. 随便双击一个 \`.mat\`，看 Inspector 里的 **Shader** 字段

**修复：**
- 如果 Shader 显示 \`None\` 或 \`Hidden/InternalErrorShader\`：材质的 shader 引用断了
- 检查 \`Assets/PluginsDeveloper/Runtime/Shaders/Lit.shader\` 是否存在
- 如果文件在但材质引用断：材质的 GUID 和 shader 的 GUID 不匹配（通常是 \`.meta\` 丢失导致）

**极端修复：** 重新 clone 一次（确保 \`.meta\` 完整）。

---

## 4. 描边问题

### 4.1 描边完全消失

**原因：** OutlineSmoothNormalsGenerator 没装，\`Lit.shader\` 的 Outline Pass 编译失败。

**修复：**
1. \`Window → Package Manager\`
2. 左上角 **+** → **Add package from git URL**
3. 输入：
   \`\`\`
   https://github.com/AleFeng/OutlineSmoothNormalsGenerator.git?path=/Packages/com.alefeng.outlinesmoothnormalsgenerator
   \`\`\`
4. 等拉取完成，Console 报错消失后，重新打开场景

### 4.2 描边错乱 / 偏移 / 断裂

**原因：** 平滑法线的"存储方式"和"存储空间"与烘焙时不一致。

**修复：**
1. 选中角色模型，看 \`OutlineSmoothNormalsGenerator\` 组件的设置
2. 对比材质面板 \`Outline → 平滑法线来源 / 存储空间\`
3. **两边必须完全一致**，不一致不会报错，但描边会错

---

## 5. 后处理失效

**现象：** Demo 场景里没有 Bloom / 径向模糊 / 故障效果等。

**快速诊断：**
1. \`Window → General → Console\` 搜索 \`FsURPPostProcessing\` 或 \`CustomPostProcess\`
2. 检查 \`Assets/PluginsDeveloper/AlePostProcessingURP/\` 下的 \`.cs\` 文件是否存在

**修复：**
- 如果源码缺失（只有 \`.meta\`）：重新从 TAProject 复制
- 如果编译报错：检查 \`AlePostProcessingURP\` 的 asmdef 引用是否完整

---

## 6. 中文乱码

**现象：** 打开 \`.hlsl\` / \`.cs\` 文件，中文注释变成 \`閫氱敤宸ュ叿\` 这种乱码。

**原因：** 文件编码不是 UTF-8（可能是 GBK / ANSI），或编辑器按错误编码打开。

**修复：**

1. **用 VSCode 打开**，右下角看编码（应该显示 \`UTF-8\`）
2. 如果显示 \`GBK\` 或其他：点它 → **Reopen with Encoding** → \`UTF-8\`
3. 如果还是乱码：文件本身被写坏了，从 git 恢复：
   \`\`\`bash
   git checkout -- Assets/PluginsDeveloper/
   \`\`\`

**预防：** 项目根目录有 \`.editorconfig\`，VSCode / Rider / VS 都支持，会自动用 UTF-8。

---

## 7. 资产占位符

**现象：** 克隆后模型、贴图全是灰色占位符，文件大小是几百字节。

**原因：** 没拉 LFS。

**修复：**
\`\`\`bash
cd SumiToon
git lfs install
git lfs pull
\`\`\`

**验证：** \`Assets\` 体积应该约 250 MB。

---

## 8. 还是解决不了

1. 收集信息：
   - Unity 版本（\`Help → About Unity\`）
   - Console 完整报错（截图或复制文本）
   - \`git status\` 输出
2. 在仓库提 Issue，附上以上信息
3. 或者对照 [THIRD-PARTY-NOTICES.md](../THIRD-PARTY-NOTICES.md) 里的原始仓库地址，去上游提 Issue
