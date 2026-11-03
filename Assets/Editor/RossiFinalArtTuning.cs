using UnityEngine;
using UnityEditor;

public class RossiFinalArtTuning
{
    [MenuItem("Tools/SumiToon/Final Art Tuning")]
    public static void Tune()
    {
        string matDir = "Assets/ProductAssets/Prefab/Character/Rossi/Materials/";
        
        // 1. 面部皮肤：提亮肤色，阴影偏冷粉，减少红晕
        Material faceMat = AssetDatabase.LoadAssetAtPath<Material>(matDir + "面部皮肤.mat");
        if (faceMat != null)
        {
            // 让脸部曝光更通透
            faceMat.SetFloat("_FloatBaseMapExposureIntensity", 1.5f);
            faceMat.SetFloat("_FloatRealtimeLightIntensity", 2.0f);
            
            // 暗部1（浅阴影）偏粉白，暗部2（深阴影）偏冷紫
            faceMat.SetColor("_ColorShade1Color", new Color(0.96f, 0.88f, 0.89f, 1f)); 
            faceMat.SetColor("_ColorShade2Color", new Color(0.75f, 0.65f, 0.75f, 1f)); 
            
            // SDF 阴影羽化调大，边缘像水彩一样晕开
            faceMat.SetFloat("_FloatFaceSDFSoftness", 0.15f);
            faceMat.SetFloat("_FloatFaceSDFIntensity", 0.85f);
            EditorUtility.SetDirty(faceMat);
        }

        // 2. 头发：开启高光，暗部偏冷灰紫（不要黑）
        Material hairMat = AssetDatabase.LoadAssetAtPath<Material>(matDir + "头发.mat");
        if (hairMat != null)
        {
            hairMat.EnableKeyword("_HIGHLIGHT_ON");
            hairMat.SetFloat("_ToggleHighLight", 1f);
            hairMat.SetFloat("_FloatHighLightSize", 0.65f); // 天使环大小
            hairMat.SetFloat("_ToggleHighLightSoft", 0f);   // 锐利的高光
            hairMat.SetColor("_ColorHighLightColor", new Color(1f, 0.98f, 0.9f, 1f)); // 偏白高光
            
            // 头发的暗部颜色调成灰紫
            hairMat.SetColor("_ColorShade1Color", new Color(0.8f, 0.75f, 0.8f, 1f));
            hairMat.SetColor("_ColorShade2Color", new Color(0.55f, 0.5f, 0.6f, 1f));
            
            // 头发边缘光的颜色
            hairMat.SetColor("_ColorRimLightColor", new Color(1f, 0.9f, 0.7f, 0.5f));
            EditorUtility.SetDirty(hairMat);
        }

        // 3. 衣服/皮裙：调整暗部颜色，增强体积感
        Material clothMat = AssetDatabase.LoadAssetAtPath<Material>(matDir + "披风白裙与皮革.mat");
        if (clothMat != null)
        {
            clothMat.SetColor("_ColorShade1Color", new Color(0.75f, 0.75f, 0.8f, 1f));
            clothMat.SetColor("_ColorShade2Color", new Color(0.5f, 0.5f, 0.6f, 1f));
            EditorUtility.SetDirty(clothMat);
        }

        // 4. 眼睛：强化高光和发光感
        Material irisMat = AssetDatabase.LoadAssetAtPath<Material>(matDir + "虹膜瞳孔.mat");
        if (irisMat != null)
        {
            irisMat.EnableKeyword("_EMISSIVE_ON");
            irisMat.SetFloat("_ToggleEmissive", 1f);
            irisMat.SetColor("_ColorEmissiveMapColor", new Color(0.5f, 0.4f, 0.1f, 1f)); // 微微发光
            EditorUtility.SetDirty(irisMat);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("<color=green>最终美术调色完成！请手动把 Matcap 图挂进对应的槽位。</color>");
    }
}
