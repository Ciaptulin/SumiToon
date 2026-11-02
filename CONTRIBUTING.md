# 贡献指南

> 本文件是 GitHub 自动识别的入口，**详细规范见 [docs/CONTRIBUTING.md](./docs/CONTRIBUTING.md)**。

## 快速上手

1. 先读 [docs/GETTING_STARTED.md](./docs/GETTING_STARTED.md)，确保能跑起来
2. 再读 [docs/ARCHITECTURE.md](./docs/ARCHITECTURE.md)，理解代码结构
3. 改代码前读 [docs/CONTRIBUTING.md](./docs/CONTRIBUTING.md)，遵守提交规范和代码规范

## 提交前自检

- [ ] Unity 能打开，Console 无红色报错
- [ ] 至少打开一个 Demo 场景，效果正常
- [ ] 修改的 `.shader` / `.hlsl` 有文件头
- [ ] 新增的魔法数字有注释
- [ ] `CHANGELOG.md` 已更新（用户可见改动）

## 提交信息

遵循 Conventional Commits：

\`\`\`
feat(shader): 新增 XXX 参数
fix(outline): 修复 XXX 问题
docs: 补全 XXX 说明
\`\`\`

## 不要提交

- \`Library/\` / \`Logs/\` / \`UserSettings/\`
- \`*.csproj\` / \`*.sln\`
- 真实邮箱（用 GitHub noreply）
- 大文件（走 LFS）

## 许可

提交的代码默认以 MIT License 授权。
第三方组件版权归原作者，不要删除任何版权声明。
