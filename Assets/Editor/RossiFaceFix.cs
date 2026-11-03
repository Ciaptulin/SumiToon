using UnityEngine;
using UnityEditor;

public class RossiFaceFix
{
    [MenuItem("Tools/SumiToon/Fix Face Colors Only")]
    public static void Fix()
    {
        string path = "Assets/ProductAssets/Prefab/Character/Rossi/Materials/面部皮肤.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        
        if (mat == null) return;

        // 1. 彻底还原基础底色和暗部颜色（洗干净，不要乱加滤镜）
        // 基础颜色设为纯白，让贴图原本的肤色透出来
        mat.SetColor("_BaseColor", Color.white); 
        mat.SetColor("_ColorBaseMapBlendColor", Color.white);
        mat.SetFloat("_BaseMapBlendColorIntensity", 0f); // 关闭染色

        // 暗部颜色设为极浅的粉白（绝不能发黑或发红）
        mat.SetColor("_ColorShade1Color", new Color(0.95f, 0.9f, 0.9f, 1f)); 
        mat.SetColor("_ColorShade2Color", new Color(0.8f, 0.75f, 0.75f, 1f)); 

        // 2. 恢复正确的光照强度
        mat.SetFloat("_FloatBaseMapExposureIntensity", 2.0f);
        mat.SetFloat("_FloatRealtimeLightIntensity", 5.0f);
        mat.SetFloat("_FloatEnvLightIntensity", 2.0f);

        // 3. 恢复你之前调好的 SDF 参数
        mat.EnableKeyword("_FACESDF_ON");
        mat.SetFloat("_ToggleFaceSDF", 1f);
        mat.SetFloat("_FloatFaceSDFIntensity", 1.0f);
        mat.SetFloat("_FloatFaceSDFThreshold", 0.5f);
        mat.SetFloat("_FloatFaceSDFSoftness", 0.05f);
        mat.SetFloat("_FloatFaceSDFDebugFrame", -1f); // 自动跟随光照

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        Debug.Log("<color=green>面部颜色和SDF参数已彻底洗白恢复！</color>");
    }
}
