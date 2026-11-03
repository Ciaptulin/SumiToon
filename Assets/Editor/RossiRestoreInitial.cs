using UnityEngine;
using UnityEditor;

public class RossiRestoreInitial
{
    [MenuItem("Tools/SumiToon/Restore Rossi To Initial")]
    public static void Restore()
    {
        string matDir = "Assets/ProductAssets/Prefab/Character/Rossi/Materials/";
        
        // 1. 面部皮肤：还原为泛白干净的肤色，描边恢复原样
        Material faceMat = AssetDatabase.LoadAssetAtPath<Material>(matDir + "面部皮肤.mat");
        if (faceMat != null)
        {
            faceMat.SetFloat("_FloatOutlineWidth", 0.003f); // 恢复原本的粗描边
            faceMat.SetColor("_ColorOutlineColor", new Color(0.28f, 0.28f, 0.28f, 1f)); // 默认深灰
            
            // 恢复你之前调的偏粉白阴影
            faceMat.SetColor("_ColorShade1Color", new Color(0.98f, 0.86f, 0.86f, 1f)); 
            faceMat.SetColor("_ColorShade2Color", new Color(0.86f, 0.70f, 0.74f, 1f)); 
            
            faceMat.SetFloat("_FloatBaseMapExposureIntensity", 1.2f);
            faceMat.SetFloat("_FloatRealtimeLightIntensity", 2.5f);
            
            // 恢复 SDF 默认参数
            faceMat.SetFloat("_FloatFaceSDFSoftness", 0.05f);
            faceMat.SetFloat("_FloatFaceSDFIntensity", 1.0f);
            EditorUtility.SetDirty(faceMat);
        }

        // 2. 头发：彻底关掉硬高光，把颜色洗干净
        Material hairMat = AssetDatabase.LoadAssetAtPath<Material>(matDir + "头发.mat");
        if (hairMat != null)
        {
            hairMat.DisableKeyword("_HIGHLIGHT_ON");
            hairMat.SetFloat("_ToggleHighLight", 0f);
            hairMat.SetColor("_ColorShade1Color", new Color(1f, 1f, 1f, 1f));
            hairMat.SetColor("_ColorShade2Color", new Color(1f, 1f, 1f, 1f));
            hairMat.SetFloat("_FloatOutlineWidth", 0.003f);
            EditorUtility.SetDirty(hairMat);
        }

        // 3. 衣服：洗掉颜色
        Material clothMat = AssetDatabase.LoadAssetAtPath<Material>(matDir + "披风白裙与皮革.mat");
        if (clothMat != null)
        {
            clothMat.SetColor("_ColorShade1Color", new Color(1f, 1f, 1f, 1f));
            clothMat.SetColor("_ColorShade2Color", new Color(1f, 1f, 1f, 1f));
            clothMat.SetFloat("_FloatOutlineWidth", 0.003f);
            EditorUtility.SetDirty(clothMat);
        }

        // 4. 眼睛：关掉自发光
        Material irisMat = AssetDatabase.LoadAssetAtPath<Material>(matDir + "虹膜瞳孔.mat");
        if (irisMat != null)
        {
            irisMat.DisableKeyword("_EMISSIVE_ON");
            irisMat.SetFloat("_ToggleEmissive", 0f);
            EditorUtility.SetDirty(irisMat);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("<color=green>材质已复原到你发对比图之前的初始状态！</color>");
    }
}
