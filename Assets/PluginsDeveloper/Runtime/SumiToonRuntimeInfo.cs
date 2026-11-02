// ============================================================
// 文件：SumiToonRuntimeInfo.cs
// 用途：运行时元信息（版本号、Shader 名称常量）
// 依赖：无
// 作者：SumiToon    创建：2026-11-02
// 许可：MIT
// ============================================================

namespace SumiToon
{
    /// <summary>
    /// SumiToon 运行时元信息。
    /// 用于版本查询、日志输出、编辑器面板显示、运行时 Shader.Find。
    /// </summary>
    public static class SumiToonRuntimeInfo
    {
        /// <summary>
        /// 项目版本号，与 CHANGELOG.md 保持一致。
        /// </summary>
        public const string Version = "0.1.0";

        /// <summary>
        /// Lit Shader 的完整名称，用于 Shader.Find。
        /// 注意：如果未来重命名 Shader，此处需同步修改。
        /// </summary>
        public const string LitShaderName = "AleToonURP/Lit";

        /// <summary>
        /// WaterPlane Shader 的完整名称，用于 Shader.Find。
        /// </summary>
        public const string WaterPlaneShaderName = "AleToonURP/Water Plane";

        /// <summary>
        /// 项目主页（可选，用于 About 面板）。
        /// </summary>
        public const string ProjectUrl = "https://github.com/<your-username>/SumiToon";
    }
}
