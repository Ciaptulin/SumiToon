using UnityEngine;
using UnityEditor;

public class RossiArtRevert
{
    [MenuItem("Tools/SumiToon/Revert Art Tuning")]
    public static void Revert()
    {
        string matDir = "Assets/ProductAssets/Prefab/Character/Rossi/Materials/";
        
        // 1. 面部皮肤：恢复中性的阴影颜色和曝光
        Material faceMat = AssetDatabase.LoadAssetAtPath<Material>(matDir + "面部皮肤.mat");
        if (faceMat != null)
        {
            faceMat.SetFloat("_FloatBaseMapExposureIntensity", 1.0f);
            faceMat.SetFloat("_FloatRealtimeLightIntensity", 2.5f);
            
            // 恢复为偏暖的肤色阴影，而不是紫红
            faceMat.SetColor("_ColorShade1Color", new Color(0.95f, 0.9f, 0.9f, 1f)); 
            faceMat.SetColor("_ColorShade2Color", new Color(0.8f, 0.7f, 0.7f, 1f)); 
            
            // 阴影边缘变硬一点
            faceMat.SetFloat("_FloatFaceSDFSoftness", 0.05f);
            faceMat.SetFloat("_FloatFaceSDFIntensity", 1.0f);
            EditorUtility.SetDirty(faceMat);
        }

        // 2. 头发：关闭强加的硬高光，恢复原本的暗部
        Material hairMat = AssetDatabase.LoadAssetAtPath<Material>(matDir + "头发.mat");
        if (hairMat != null)
        {
            hairMat.DisableKeyword("_HIGHLIGHT_ON");
            hairMat.SetFloat("_ToggleHighLight", 0f);
            
            hairMat.SetColor("_ColorShade1Color", new Color(0.9f, 0.85f, 0.8f, 1f));
            hairMat.SetColor("_ColorShade2Color", new Color(0.7f, 0.6f, 0.5f, 1f));
            
            hairMat.SetColor("_ColorRimLightColor", new Color(1f, 1f, 1f, 0.3f));
            EditorUtility.SetDirty(hairMat);
        }

        // 3. 衣服
        Material clothMat = AssetDatabase.LoadAssetAtPath<Material>(matDir + "披风白裙与皮革.mat");
        if (clothMat != null)
        {
            clothMat.SetColor("_ColorShade1Color", new Color(0.85f, 0.85f, 0.85f, 1f));
            clothMat.SetColor("_ColorShade2Color", new Color(0.65f, 0.65f, 0.65f, 1f));
            EditorUtility.SetDirty(clothMat);
        }

        // 4. 眼睛：关闭自发光
        Material irisMat = AssetDatabase.LoadAssetAtPath<Material>(matDir + "虹膜瞳孔.mat");
        if (irisMat != null)
        {
            irisMat.DisableKeyword("_EMISSIVE_ON");
            irisMat.SetFloat("_ToggleEmissive", 0f);
            EditorUtility.SetDirty(irisMat);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("<color=green>美术效果已回退到基础光影状态。</color>");
    }
}
