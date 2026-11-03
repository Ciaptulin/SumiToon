using UnityEngine;

namespace SumiToon
{
    [ExecuteAlways]
    [AddComponentMenu("SumiToon/Face SDF Light Driver")]
    public class FaceSDFLightDriver : MonoBehaviour
    {
        [Header("光源")]
        [Tooltip("用于 SDF 阴影的主光。留空则自动取第一个 Directional Light")]
        public Light mainLight;

        private static readonly int LightDirLocalId = Shader.PropertyToID("_FaceSDFLightDirLocal");

        private void Reset()
        {
            mainLight = FindMainLight();
        }

        private void OnEnable()
        {
            if (mainLight == null) mainLight = FindMainLight();
        }

        private void LateUpdate()
        {
            if (mainLight == null) mainLight = FindMainLight();

            // 获取主光的照射方向（从光源指向物体的方向）
            Vector3 lightDirWS = mainLight != null 
                ? (-mainLight.transform.forward).normalized 
                : Vector3.forward;

            // 将世界空间的光照方向，转换到模型的本地空间（Object Space）
            // 这样无论模型怎么转身，Shader 里拿到的永远是相对于角色的光向
            Vector3 lightDirLocal = transform.InverseTransformDirection(lightDirWS);

            Shader.SetGlobalVector(LightDirLocalId, new Vector4(lightDirLocal.x, lightDirLocal.y, lightDirLocal.z, 1f));
        }

        private static Light FindMainLight()
        {
            Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (var l in lights)
            {
                if (l.type == LightType.Directional && l.isActiveAndEnabled)
                    return l;
            }
            return null;
        }
    }
}
