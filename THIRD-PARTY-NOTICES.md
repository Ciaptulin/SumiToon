# Third-Party Notices

本项目包含以下第三方组件。各组件的版权归其原作者所有，本项目按原许可使用。

---

## 1. AleToonURP

- **来源**：https://github.com/AleFeng/AleToonURP
- **版本**：v1.4.0
- **作者**：AleFeng
- **许可**：MIT License
- **用途**：NPR 卡通渲染管线与 Shader（Lit / WaterPlane）的实现基础
- **位置**：`Packages/com.sumitoon.urp/`
- **修改说明**：本项目在其基础上进行了代码规范化（文件头、注释、格式），核心逻辑未改动

### MIT License (AleToonURP)

Copyright (c) AleFeng

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

---

## 2. OutlineSmoothNormalsGenerator

- **来源**：https://github.com/AleFeng/OutlineSmoothNormalsGenerator
- **版本**：1.8.1（建议）
- **作者**：AleFeng
- **许可**：MIT License
- **用途**：烘焙平滑法线，供外描边 Pass 使用（OSN_GetSmoothNormalOS / OSN_ApplyOutlineOffset）
- **依赖方式**：Unity Package Manager（Git URL）

---

## 3. Unity Universal Render Pipeline (URP)

- **来源**：Unity Technologies
- **版本**：16.0.6
- **许可**：Unity Companion License
- **用途**：渲染管线基础设施

---

## 4. 角色模型与贴图（ProductAssets）

- **来源**：米哈游《崩坏：星穹铁道》（卡芙卡）、《原神》（可莉）
- **用途**：**仅用于本地学习与研究，不用于商业发布**
- **版权声明**：角色形象、模型、贴图版权归 miHoYo 所有，本项目不主张任何权利
- **注意**：如本项目对外发布，需**移除或替换** `Assets/ProductAssets/` 下的所有角色资产

---

## 5. 场景资产（Factory / Forest）

- **来源**：来自 AleToonURP 原项目示例或第三方素材包（需核对原始许可）
- **用途**：学习演示
- **注意**：对外发布前需确认原始许可
