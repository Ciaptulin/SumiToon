using UnityEngine;
using UnityEditor;

public class RossiApplyKafkaFaceParams
{
    [MenuItem("Tools/SumiToon/Apply Kafka Face Parameters to Rossi")]
    public static void Apply()
    {
        string matPath = "Assets/ProductAssets/Prefab/Character/Rossi/Materials/面部皮肤.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        
        if (mat == null) return;

        // 1. 彻底关掉自发光（参考作者，绝不瞎发光）
        mat.DisableKeyword("_EMISSIVE_ON");
        mat.SetFloat("_ToggleEmissive", 0f);
        mat.SetColor("_ColorEmissiveMapColor", new Color(0f, 0f, 0f, 1f));

        // 2. 完全照抄 Kafka 的光照强度与曝光
        mat.SetFloat("_FloatRealtimeLightIntensity", 5.0f);
        mat.SetFloat("_FloatBaseMapExposureIntensity", 1.0f);
        mat.SetFloat("_FloatEnvLightIntensity", 2.0f);

        // 3. 完全照抄 Kafka 的暗部颜色（偏浅肉色，不要死灰）
        mat.SetColor("_ColorShade1Color", new Color(1.0f, 0.94f, 0.92f, 1f)); 
        mat.SetColor("_ColorShade2Color", new Color(0.95f, 0.69f, 0.60f, 1f)); 

        // 4. 照抄 Kafka 的奶油般羽化（这是关键！）
        mat.SetFloat("_FloatBrightShade1Step", 0.5f);
        mat.SetFloat("_FloatBrightShade1Blur", 0.784f); 
        mat.SetFloat("_FloatShade1Shade2Step", 0.45f);
        mat.SetFloat("_FloatShade1Shade2Blur", 1.0f);

        // 5. 开启水平光照锁定（Kafka 是开的，这能让脸部阴影更平）
        mat.SetFloat("_ToggleLightHorLockBaseMap", 1.0f);

        // 6. SDF 参数设为柔化状态
        mat.SetFloat("_FloatFaceSDFIntensity", 1.0f);
        mat.SetFloat("_FloatFaceSDFThreshold", 0.4f); // 稍微往下调一点
        mat.SetFloat("_FloatFaceSDFSoftness", 0.1f);  // 稍微拉大羽化

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        Debug.Log("<color=green>已完全套用 Kafka 的官方光影参数，自发光已彻底关闭！</color>");
    }
}
