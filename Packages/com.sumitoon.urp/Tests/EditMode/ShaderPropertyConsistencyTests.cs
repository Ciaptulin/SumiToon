// ============================================================
// 文件：ShaderPropertyConsistencyTests.cs
// 用途：校验 Lit.shader 的 Properties 块与 LitPropInput.hlsl 的 CBUFFER
//      属性名是否一致，防止改一处忘另一处导致材质参数失效
// 依赖：NUnit、UnityEditor
// 作者：SumiToon    创建：2026-11-02
// 许可：MIT
// ============================================================

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace SumiToon.Tests
{
    /// <summary>
    /// Lit Shader 的 Properties 与 CBUFFER 一致性校验。
    /// 这两处必须同时存在同名属性，否则材质面板改了值但 Shader 读不到（或反之）。
    /// </summary>
    public class ShaderPropertyConsistencyTests
    {
        private const string ShaderPath = "Packages/com.sumitoon.urp/Runtime/Shaders/Lit.shader";
        private const string PropInputPath = "Packages/com.sumitoon.urp/Runtime/Shaders/LitPropInput.hlsl";

        [Test]
        public void ShaderFile_Exists()
        {
            Assert.IsTrue(File.Exists(ShaderPath), $"找不到 {ShaderPath}");
        }

        [Test]
        public void PropInputFile_Exists()
        {
            Assert.IsTrue(File.Exists(PropInputPath), $"找不到 {PropInputPath}");
        }

        [Test]
        public void PropertiesBlock_HasNoDuplicateNames()
        {
            var names = ParsePropertiesNames(File.ReadAllText(ShaderPath));
            var seen = new HashSet<string>();
            var dup = new List<string>();
            foreach (var n in names)
            {
                if (!seen.Add(n)) dup.Add(n);
            }
            Assert.IsEmpty(dup, $"Properties 中存在重复属性名: {string.Join(", ", dup)}");
        }

        [Test]
        public void CBufferBlock_HasNoDuplicateNames()
        {
            var names = ParseCBufferNames(File.ReadAllText(PropInputPath));
            var seen = new HashSet<string>();
            var dup = new List<string>();
            foreach (var n in names)
            {
                if (!seen.Add(n)) dup.Add(n);
            }
            Assert.IsEmpty(dup, $"CBUFFER 中存在重复属性名: {string.Join(", ", dup)}");
        }

        [Test]
        public void CBufferProperties_AllDeclaredInShaderProperties()
        {
            var shaderProps = new HashSet<string>(ParsePropertiesNames(File.ReadAllText(ShaderPath)));
            var cbufferProps = ParseCBufferNames(File.ReadAllText(PropInputPath));

            // _ST 是 Unity 自动生成的贴图平铺/偏移变量，不需要在 Properties 里声明
            var missing = new List<string>();
            foreach (var n in cbufferProps)
            {
                if (n.EndsWith("_ST")) continue;
                if (!shaderProps.Contains(n)) missing.Add(n);
            }

            Assert.IsEmpty(missing,
                $"以下 CBUFFER 属性在 Lit.shader 的 Properties 中不存在，材质面板无法设置: {string.Join(", ", missing)}");
        }

        [Test]
        public void ShaderProperties_NamingFollowsPrefixConvention()
        {
            var names = ParsePropertiesNames(File.ReadAllText(ShaderPath));
            var violations = new List<string>();

            foreach (var n in names)
            {
                // 允许的前缀：_Tex / _Color / _Float / _Toggle / _Int，以及 Unity 内置的 _BaseMap / _BumpMap
                if (n == "_BaseMap" || n == "_BumpMap") continue;
                if (n.StartsWith("_Tex") || n.StartsWith("_Color") || n.StartsWith("_Float")
                    || n.StartsWith("_Toggle") || n.StartsWith("_Int")) continue;
                violations.Add(n);
            }

            Assert.IsEmpty(violations,
                $"以下属性不符合命名规范（应为 _Tex/_Color/_Float/_Toggle/_Int 前缀）: {string.Join(", ", violations)}");
        }

        /// <summary>
        /// 从 Shader 的 Properties 块中提取属性名（形如 _Name("显示名", Type)）
        /// </summary>
        private static List<string> ParsePropertiesNames(string shaderText)
        {
            var result = new List<string>();

            int propStart = shaderText.IndexOf("Properties", System.StringComparison.Ordinal);
            if (propStart < 0) return result;

            int braceStart = shaderText.IndexOf('{', propStart);
            if (braceStart < 0) return result;

            int depth = 0;
            int braceEnd = -1;
            for (int i = braceStart; i < shaderText.Length; i++)
            {
                if (shaderText[i] == '{') depth++;
                else if (shaderText[i] == '}')
                {
                    depth--;
                    if (depth == 0) { braceEnd = i; break; }
                }
            }
            if (braceEnd < 0) return result;

            string block = shaderText.Substring(braceStart, braceEnd - braceStart);
            var matches = Regex.Matches(block, @"^s*(_w+)s*(", RegexOptions.Multiline);
            foreach (Match m in matches)
            {
                result.Add(m.Groups[1].Value);
            }
            return result;
        }

        /// <summary>
        /// 从 HLSL 的 CBUFFER_START(UnityPerMaterial) ... CBUFFER_END 中提取变量名
        /// </summary>
        private static List<string> ParseCBufferNames(string hlslText)
        {
            var result = new List<string>();

            int start = hlslText.IndexOf("CBUFFER_START(UnityPerMaterial)", System.StringComparison.Ordinal);
            if (start < 0) return result;

            int end = hlslText.IndexOf("CBUFFER_END", start, System.StringComparison.Ordinal);
            if (end < 0) return result;

            string block = hlslText.Substring(start, end - start);
            // 匹配 half/float/real/int/uint 开头的变量声明，提取变量名
            var matches = Regex.Matches(block,
                @"^s*(?:half|float|real|int|uint)(?:[1-4])?s+(_w+)s*;",
                RegexOptions.Multiline);
            foreach (Match m in matches)
            {
                result.Add(m.Groups[1].Value);
            }
            return result;
        }
    }
}
