# 新手入门（GETTING STARTED）

> 目标：**10 分钟内**从零开始，在 Unity 里看到 Demo 场景的渲染效果。
> 如果你只是想读懂代码，跳到 [ARCHITECTURE.md](./ARCHITECTURE.md)。

---

## 0. 前置条件

| 依赖 | 版本 | 说明 |
|---|---|---|
| Unity | **2023.2.20f1** | 版本不一致会触发资产升级，可能破坏场景 |
| Git | 2.x+ | 必须支持 LFS |
| Git LFS | 3.x+ | 252 MB 资产全部走 LFS，不装拉不下来 |
| 磁盘空间 | **≥ 5 GB** | Library 缓存会占 1~2 GB |

**检查 LFS 是否安装：**

\`\`\`bash
git lfs version
# 应该输出 git-lfs/3.x.x
\`\`\`

---

## 1. 克隆项目

\`\`\`bash
git clone <repo-url> SumiToon
cd SumiToon

# 拉取 LFS 资产（必须！否则模型、贴图全是占位符）
git lfs pull
\`\`\`

**验证资产是否拉全：**

\`\`\`bash
# Assets 体积应该在 250 MB 左右
du -sh Assets/
\`\`\`

---

## 2. 用 Unity 打开

1. Unity Hub → **Add** → 选择 \`SumiToon\` 目录
2. **必须用 2023.2.20f1 打开**（Hub 里可以指定版本）
3. 首次导入会持续 **3~5 分钟**，控制台会滚动输出，属正常现象

---

## 3. 首次打开后的必做检查

### 3.1 确认渲染管线正确

1. 打开 \`Project Settings → Graphics\`
2. **Scriptable Render Pipeline Settings** 应该指向 \`Assets/URPSettings/AleToonUPR_Settings\`
3. 如果为空或指向别处：手动选上（否则所有材质都会变粉红）

### 3.2 确认 OutlineSmoothNormalsGenerator 已安装

1. 打开 \`Window → Package Manager\`
2. 在 \`In Project\` 下拉里选 \`Packages: My Registries\` 或 \`Packages: Unity Registry\`
3. 搜索 \`OutlineSmoothNormalsGenerator\`，应该能看到
4. 如果没有：点左上角 **+** → **Add package from git URL**，输入：

   \`\`\`
   https://github.com/AleFeng/OutlineSmoothNormalsGenerator.git?path=/Packages/com.alefeng.outlinesmoothnormalsgenerator
   \`\`\`

5. 装完后 \`Assets/PluginsDeveloper/Runtime/Shaders/LitOutline.hlsl\` 应该不再报错

### 3.3 确认 Console 无红色报错

打开 \`Window → General → Console\`，清空后重新加载一次项目（Ctrl+R）。**如果有红色报错**，对照 [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) 处理。

---

## 4. 打开 Demo 场景

在 \`Assets/Scenes/\` 下有三个场景，任选一个双击打开：

| 场景 | 内容 | 看什么 |
|---|---|---|
| **DemoKafka.unity** | 卡芙卡角色 + 工厂场景 | Lit Shader 的完整效果：三色阶、描边、Rim、高光、MatCap |
| **DemoKlee.unity** | 可莉角色 + 森林场景 | 同上，角色风格不同 |
| **DemoWater.unity** | 水面场景 | WaterPlane Shader：深度水色、波形、折射、反射、泡沫 |

打开后按 **Play**，用 \`WASD + 鼠标\` 移动相机（场景里挂了 \`AleCameraRover\` 组件）。

---

## 5. 验证清单（5 项都通过说明环境正常）

- [ ] 场景能正常渲染，角色不是粉色
- [ ] 角色有**黑色描边**（Outline）
- [ ] 角色**背光侧有边缘光**（Rim Light），且比迎光侧暗
- [ ] 高光**有形状**（不是圆点，是色块/条状）
- [ ] DemoWater 场景里水面有**波纹动画**和**岸边泡沫**

**任意一项不通过** → 跳到 [TROUBLESHOOTING.md](./TROUBLESHOOTING.md)。

---

## 6. 接下来做什么

| 你想做的事 | 看哪份文档 |
|---|---|
| 调材质参数做出不同风格 | [UserManual.md](../Document~/UserManual.md)（每个参数的说明） |
| 读懂代码、理解渲染流程 | [ARCHITECTURE.md](./ARCHITECTURE.md) |
| 改代码、提交贡献 | [CONTRIBUTING.md](./CONTRIBUTING.md) |
| 遇到报错 / 效果不对 | [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) |
| 了解第三方组件与许可 | [THIRD-PARTY-NOTICES.md](../THIRD-PARTY-NOTICES.md) |

---

## 7. 快速理解这个项目（30 秒版）

\`\`\`
这是一个基于 AleToonURP 的 NPR（二次元卡通）渲染项目。
核心是两个 Shader：
- Lit.shader         → 角色/场景的卡通渲染（三色阶 + 描边 + Rim + 高光 + MatCap + 自发光）
- WaterPlane.shader  → 风格化水面（深度水色 + 波形 + 折射 + 反射 + 泡沫）

代码结构：
- Assets/PluginsDeveloper/Runtime/Shaders/   ← Shader + HLSL
- Assets/PluginsDeveloper/Editor/            ← 自定义材质面板（ShaderGUI）
- Assets/PluginsDeveloper/Example/           ← 示例材质与贴图
- Assets/ProductAssets/                      ← 角色模型和场景（学习用）
- Assets/Scenes/                             ← 三个 Demo 场景
- Assets/URPSettings/                        ← 管线配置和 Volume
- Learning/                                  ← 学习笔记（不进 Unity 编译）
\`\`\`
