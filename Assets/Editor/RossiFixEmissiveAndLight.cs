using UnityEngine;
using UnityEditor;

public class RossiFixEmissiveAndLight
{
    [MenuItem("Tools/SumiToon/Fix Face Emissive & Light")]
    public static void Fix()
    {
        string matPath = "Assets/ProductAssets/Prefab/Character/Rossi/Materials/面部皮肤.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        
        if (mat == null)
        {
            Debug.LogError("找不到面部材质！");
            return;
        }

        // 1. 恢复发光的灵魂（点亮二极管）
        mat.EnableKeyword("_EMISSIVE_ON");
        mat.SetFloat("_ToggleEmissive", 1f);
        // 发光颜色设为淡淡的肉粉色，微微发光，防止脸部死黑
        mat.SetColor("_ColorEmissiveMapColor", new Color(1.0f, 0.85f, 0.85f, 1.0f));

        // 2. 恢复被乱改的基础光照和曝光
        mat.SetFloat("_FloatRealtimeLightIntensity", 5.0f);
        mat.SetFloat("_FloatBaseMapExposureIntensity", 2.0f);
        mat.SetFloat("_FloatEnvLightIntensity", 2.0f);

        // 3. 洗干净底色和暗部颜色（恢复成原版健康的粉白肤色）
        mat.SetColor("_BaseColor", Color.white); 
        mat.SetColor("_ColorBaseMapBlendColor", Color.white);
        mat.SetFloat("_BaseMapBlendColorIntensity", 0f); // 关掉脏色混合
        
        mat.SetColor("_ColorShade1Color", new Color(0.95f, 0.88f, 0.88f, 1f)); 
        mat.SetColor("_ColorShade2Color", new Color(0.8f, 0.75f, 0.75f, 1f)); 

        // 4. SDF 参数保持你最后一次调好的状态（只把强度拉回正常）
        mat.SetFloat("_FloatFaceSDFIntensity", 1.0f);
        mat.SetFloat("_FloatFaceSDFSoftness", 0.05f);

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        Debug.Log("<color=green>面部自发光已点亮！光照与底色已彻底复原！</color>");
    }
}
