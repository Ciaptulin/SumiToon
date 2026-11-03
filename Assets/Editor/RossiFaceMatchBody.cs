using UnityEngine;
using UnityEditor;

public class RossiFaceMatchBody
{
    [MenuItem("Tools/SumiToon/Match Face Light to Body")]
    public static void Match()
    {
        string matPath = "Assets/ProductAssets/Prefab/Character/Rossi/Materials/面部皮肤.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        
        if (mat == null) return;

        // 1. 完全照抄身体皮肤的曝光与光照参数
        mat.SetFloat("_FloatBaseMapExposureIntensity", 2.0f);
        mat.SetFloat("_FloatRealtimeLightIntensity", 5.0f);
        mat.SetFloat("_FloatEnvLightIntensity", 2.0f);
        mat.SetFloat("_FloatGlobalExposureIntensity", 1.0f);

        // 2. 关闭水平光照锁定（这是脸黑的核心原因！）
        mat.SetFloat("_ToggleLightHorLockBaseMap", 0.0f);

        // 3. 恢复正常的色阶分布（照抄身体，不要离谱的 0.784 羽化）
        mat.SetFloat("_FloatBrightShade1Step", 0.5f);
        mat.SetFloat("_FloatBrightShade1Blur", 0.2f);
        mat.SetFloat("_FloatShade1Shade2Step", 0.2f);
        mat.SetFloat("_FloatShade1Shade2Blur", 0.5f);

        // 4. 暗部颜色用稍微带有血色的灰色，避免死黑
        mat.SetColor("_ColorShade1Color", new Color(0.8f, 0.75f, 0.75f, 1f)); 
        mat.SetColor("_ColorShade2Color", new Color(0.6f, 0.5f, 0.5f, 1f)); 

        // 5. 彻底关掉 SDF 和自发光（先看纯底色）
        mat.SetFloat("_ToggleFaceSDF", 0.0f);
        mat.DisableKeyword("_FACESDF_ON");
        mat.SetFloat("_ToggleEmissive", 0.0f);
        mat.DisableKeyword("_EMISSIVE_ON");

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        Debug.Log("<color=green>已完全对齐身体的光照！SDF已暂时关闭，请查看脸部是否亮起。</color>");
    }
}
