// ============================================================
// 文件：MmdBlendShapeController.cs
// 用途：驱动 MMD 日文命名 blend shape（まばたき / 笑い / あ / い / う / え / お 等）
// 依赖：SumiToon.Runtime.asmdef
// 说明：洛茜工程 41 个表情全部是日文命名，本脚本提供：
//       1) 预设名映射（眨眼、笑、口型）
//       2) 呼吸/眨眼自动循环（可选）
//       3) 运行时按名称或索引设置权重
// 许可：MIT
// ============================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SumiToon
{
    [ExecuteAlways]
    [AddComponentMenu("SumiToon/MMD BlendShape Controller")]
    public class MmdBlendShapeController : MonoBehaviour
    {
        [Header("目标")]
        [Tooltip("要驱动的 SkinnedMeshRenderer。留空则自动在自身/子物体查找")]
        public SkinnedMeshRenderer target;

        [Header("自动眨眼")]
        public bool autoBlink = true;
        [Tooltip("两次眨眼间隔（秒）")]
        public Vector2 blinkInterval = new Vector2(2.5f, 5.5f);
        [Tooltip("单次眨眼时长（秒）")]
        public float blinkDuration = 0.12f;
        [Tooltip("眨眼表情名（MMD 标准）")]
        public string blinkShapeName = "まばたき";

        [Header("口型")]
        [Tooltip("口型表情名前缀。MMD 里通常是「あ」「い」「う」「え」「お」")]
        public string[] mouthShapeNames = { "あ", "い", "う", "え", "お" };

        // ---- 运行时缓存 ----
        private Mesh _mesh;
        private Dictionary<string, int> _nameToIndex = new Dictionary<string, int>(StringComparer.Ordinal);

        // 眨眼状态
        private float _nextBlinkTime;
        private float _blinkStartTime = -1f;

        private void OnEnable()
        {
            ResolveTarget();
            BuildNameIndex();
            ScheduleNextBlink();
        }

        private void ResolveTarget()
        {
            if (target != null) return;
            target = GetComponent<SkinnedMeshRenderer>();
            if (target == null) target = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        private void BuildNameIndex()
        {
            _nameToIndex.Clear();
            if (target == null || target.sharedMesh == null) return;
            _mesh = target.sharedMesh;
            for (int i = 0; i < _mesh.blendShapeCount; i++)
            {
                string n = _mesh.GetBlendShapeName(i);
                if (!_nameToIndex.ContainsKey(n)) _nameToIndex[n] = i;
            }
        }

        private void Update()
        {
            if (!Application.isPlaying) return;
            if (autoBlink) UpdateBlink(Time.time);
        }

        // ---------------- 公开 API ----------------

        /// <summary>按名称设置权重（0..1）</summary>
        public bool SetShape(string name, float weight)
        {
            if (_nameToIndex.Count == 0) BuildNameIndex();
            if (!_nameToIndex.TryGetValue(name, out int idx)) return false;
            target.SetBlendShapeWeight(idx, Mathf.Clamp01(weight) * 100f);
            return true;
        }

        /// <summary>按名称读取当前权重（0..1）</summary>
        public float GetShape(string name)
        {
            if (_nameToIndex.Count == 0) BuildNameIndex();
            if (!_nameToIndex.TryGetValue(name, out int idx)) return 0f;
            return target.GetBlendShapeWeight(idx) / 100f;
        }

        /// <summary>口型：设置第 i 个口型权重，其余归零</summary>
        public void SetMouth(int index, float weight)
        {
            for (int i = 0; i < mouthShapeNames.Length; i++)
                SetShape(mouthShapeNames[i], i == index ? weight : 0f);
        }

        /// <summary>强制眨眼一次（不等待自动循环）</summary>
        public void TriggerBlink()
        {
            _blinkStartTime = Time.time;
        }

        /// <summary>返回所有表情名（调试用）</summary>
        public string[] GetAllShapeNames()
        {
            if (_mesh == null && target != null) _mesh = target.sharedMesh;
            if (_mesh == null) return Array.Empty<string>();
            var arr = new string[_mesh.blendShapeCount];
            for (int i = 0; i < arr.Length; i++) arr[i] = _mesh.GetBlendShapeName(i);
            return arr;
        }

        // ---------------- 眨眼 ----------------

        private void ScheduleNextBlink()
        {
            _nextBlinkTime = Time.time + UnityEngine.Random.Range(blinkInterval.x, blinkInterval.y);
        }

        private void UpdateBlink(float now)
        {
            // 触发新的眨眼
            if (_blinkStartTime < 0f && now >= _nextBlinkTime)
                _blinkStartTime = now;

            if (_blinkStartTime < 0f) return;

            float t = (now - _blinkStartTime) / Mathf.Max(0.01f, blinkDuration);

            // 0 → 1 → 0 的三角形曲线
            float w = t < 0.5f ? (t * 2f) : (1f - (t - 0.5f) * 2f);
            w = Mathf.Clamp01(w);

            SetShape(blinkShapeName, w);

            if (t >= 1f)
            {
                SetShape(blinkShapeName, 0f);
                _blinkStartTime = -1f;
                ScheduleNextBlink();
            }
        }
    }
}
