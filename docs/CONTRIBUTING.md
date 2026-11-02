# 贡献指南（CONTRIBUTING）

> 目标：让每次提交都可追溯、可 review、可回滚。

---

## 1. 提交前自检

- [ ] Unity 能正常打开，Console 无红色报错
- [ ] 至少打开一个 Demo 场景，效果正常
- [ ] 修改的 \`.hlsl\` / \`.shader\` 文件有文件头（用途 / 依赖 / 作者 / 许可）
- [ ] 新增的魔法数字有注释说明
- [ ] \`CHANGELOG.md\` 已更新（如果是用户可见的改动）

---

## 2. 提交信息规范（Conventional Commits）

\`\`\`
<type>(<scope>): <subject>

<body>

<footer>
\`\`\`

### type（必填）

| type | 用途 | 例子 |
|---|---|---|
| \`feat\` | 新功能 | \`feat(shader): 新增边缘光内部距离参数\` |
| \`fix\` | 修 bug | \`fix(outline): 修复硬边模型描边断裂\` |
| \`docs\` | 文档 | \`docs: 补全 ARCHITECTURE 阅读路线\` |
| \`style\` | 格式（不影响逻辑） | \`style: 统一缩进为 4 空格\` |
| \`refactor\` | 重构（不改功能） | \`refactor(litmain): 拆分三色阶为独立函数\` |
| \`perf\` | 性能优化 | \`perf(matcap): 减少一次贴图采样\` |
| \`test\` | 测试 | \`test: 新增 Shader 属性完整性测试\` |
| \`chore\` | 构建/配置 | \`chore: 更新 .gitignore\` |

### scope（可选，推荐）

用目录或模块名：\`shader\` / \`outline\` / \`rimlight\` / \`matcap\` / \`water\` / \`postfx\` / \`gui\` / \`docs\` / \`build\`

### subject（必填）

- 中文或英文都行，**团队统一即可**
- 不超过 50 字符
- 不加句号
- 用祈使句（"修复" 而不是 "修复了"）

### 例子

\`\`\`
feat(rimlight): 新增 _FloatRimLightInsideDistance 参数

- 在 LitPropInput.hlsl 的 CBUFFER 增加参数
- 在 LitMain.hlsl 的边缘光块加入距离重映射
- 更新 UserManual.md 的参数说明
- ShaderGUI 增加滑块（范围 0~0.99，防止除零）

Closes #12
\`\`\`

---

## 3. 代码规范

### 3.1 文件头（每个 .shader / .hlsl 必须有）

\`\`\`hlsl
// ============================================================
// 文件：LitMain.hlsl
// 用途：AleToonURP Lit 主片元逻辑（裁剪/基础色三色阶/高光/Rim/MatCap/自发光）
// 依赖：LitPropInput.hlsl（CBUFFER）、Function.hlsl（RotateUV）
// 作者：xxx    最后修改：2026-11-02
// 许可：MIT
// ============================================================
\`\`\`

### 3.2 命名规范

| 类型 | 规则 | 正例 | 反例 |
|---|---|---|---|
| Shader 属性-贴图 | \`_Tex\` 前缀 | \`_TexRimLightMaskMap\` | \`_RimMask\` |
| Shader 属性-颜色 | \`_Color\` 前缀 | \`_ColorShade1Color\` | \`_ShadeColor1\` |
| Shader 属性-浮点 | \`_Float\` 前缀 | \`_FloatRimLightIntensity\` | \`_RimIntensity\` |
| Shader 属性-开关 | \`_Toggle\` 前缀 | \`_ToggleRimLight\` | \`_RimOn\` |
| Shader 属性-枚举 | \`_Int\` 前缀 | \`_IntClipType\` | \`_ClipType\` |
| 关键字 | 全大写 + 下划线 | \`_RIMLIGHT_ON\` | \`RIM_ON\` |
| 全局变量 | 小驼峰 | \`viewDirWS\` | \`ViewDir\` |
| 函数 | 大驼峰 | \`RotateUV\` | \`rotate_uv\` |

**空间后缀（必须）：**
- \`WS\` = 世界空间
- \`OS\` = 对象空间
- \`VS\` = 观察空间
- \`CS\` = 裁剪空间
- \`TS\` = 切线空间

**反例：** \`float3 normal\`（看不出在哪个空间，是 Shader 最常见的 bug 来源）
**正例：** \`float3 normalWS\` / \`float3 normalDirTS\`

### 3.3 魔法数字必须有解释

\`\`\`hlsl
// 反例
float x = _FloatIntensity * 0.1;

// 正例
// 0.1 为强度基准：让默认参数 1 时光照接近 0.1，配合曝光调到 1.0
float x = _FloatIntensity * 0.1;
\`\`\`

### 3.4 防 NaN / 防除零保护

凡是 \`pow\` / 除法 / \`smoothstep\` 的参数来自用户滑块，必须加保护：

\`\`\`hlsl
// pow 指数下限
pow(saturate(1 - NdotV), max(5 - _FresnelFactor * 0.5, 1e-3))

// smoothstep edge0 != edge1
smoothstep(threshold, threshold + max(_Blur, 1e-4), value)

// 归一化重映射分母
(x - d) / max(1 - d, 1e-4)
\`\`\`

### 3.5 精度规范

| 类型 | 用途 |
|---|---|
| \`float\` | 位置、UV、世界坐标、矩阵 |
| \`half\` | 颜色、光照强度、法线（移动端） |
| \`real\` | URP 推荐，按平台自动切（新代码优先用） |

### 3.6 格式化

- 缩进 **4 空格**（不混用 Tab）
- 大括号不换行：\`if (x) {\`
- 行内注释：\`// 内容\`（斜杠后一个空格）
- 分区用 ASCII：\`// ===== 分区名 =====\`（**不用 Unicode 框线**，跨编码会乱码）
- 文件末尾留空行

---

## 4. 变体管理

- **材质级开关**用 \`shader_feature_local\`
- **运行时切换**用 \`multi_compile\`
- **互斥的开关合并成枚举式声明**（避免 2^N 爆炸）：

\`\`\`hlsl
// 正确：3 个变体
#pragma shader_feature_local _CLIP_OFF _CLIP_DITHER _CLIP_ALPHA

// 错误：理论 8 个组合
#pragma shader_feature_local _CLIP_OFF
#pragma shader_feature_local _CLIP_DITHER
#pragma shader_feature_local _CLIP_ALPHA
\`\`\`

---

## 5. Git 流程

### 5.1 分支

\`\`\`bash
# 从 master 拉新分支
git checkout -b feat/rimlight-distance

# 开发完成后
git add .
git commit -m "feat(rimlight): 新增内部距离参数"
git push origin feat/rimlight-distance
# 提 Pull Request
\`\`\`

### 5.2 大文件

**所有二进制资产必须走 LFS**（\`.fbx\` / \`.png\` / \`.tga\` / \`.exr\` 等已配置）。
**不要提交以下内容**（已在 \`.gitignore\` 排除）：
- \`Library/\` / \`Logs/\` / \`UserSettings/\`
- \`*.csproj\` / \*.sln\`（Unity 自动生成）
- \`.vscode/\` / \`.idea/\`

### 5.3 提交粒度

- **一个 commit 只做一件事**：修 bug 和加功能不要混
- **代码 + 文档一起提交**：改了参数就同步更新 \`UserManual.md\`
- **不要提交注释掉的代码**：用 git 记录历史，不用注释保留

---

## 6. 文档更新

改了以下内容，必须同步更新对应文档：

| 改动 | 更新文档 |
|---|---|
| 新增 Shader 参数 | \`Document~/UserManual.md\` |
| 新增效果 | \`docs/ARCHITECTURE.md\` 的阅读路线 + \`README.md\` 特性列表 |
| 改了管线配置 | \`docs/GETTING_STARTED.md\` 的检查步骤 |
| 引入新第三方库 | \`THIRD-PARTY-NOTICES.md\` + \`LICENSE\` |
| 修复了常见问题 | \`docs/TROUBLESHOOTING.md\` |

---

## 7. 许可与版权

- 本项目代码：MIT
- 第三方组件：见 [THIRD-PARTY-NOTICES.md](../THIRD-PARTY-NOTICES.md)
- \`Assets/ProductAssets/\` 下的角色资产：**版权归 miHoYo，仅学习用，禁止商用**

**提交时不要删除任何原作者的版权声明**（文件头里的"作者：AleFeng"）。
