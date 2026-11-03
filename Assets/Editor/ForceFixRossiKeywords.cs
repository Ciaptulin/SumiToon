using UnityEngine;
using UnityEditor;

public class ForceFixRossiKeywords
{
    [MenuItem("Tools/SumiToon/Force Fix FaceSDF Keyword")]
    public static void Fix()
    {
        string matPath = "Assets/ProductAssets/Prefab/Character/Rossi/Materials/面部皮肤.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        
        if (mat == null)
        {
            Debug.LogError("找不到面部皮肤材质！");
            return;
        }

        // 强制开启 SDF 开关和关键字
        mat.SetFloat("_ToggleFaceSDF", 1f);
        mat.EnableKeyword("_FACESDF_ON");
        
        // 顺便把普通的光照关键字也开一下，免得黑
        mat.EnableKeyword("_SURFACETYPE_OPAQUE");
        mat.EnableKeyword("_CLIP_OFF");
        mat.EnableKeyword("_OUTLINE_ON");
        mat.EnableKeyword("_RIMLIGHT_ON");

        // 给一个默认的阈值和强度，保证一开就能看到阴影
        mat.SetFloat("_FloatFaceSDFIntensity", 1.0f);
        mat.SetFloat("_FloatFaceSDFThreshold", 0.5f);
        mat.SetFloat("_FloatFaceSDFSoftness", 0.05f);

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("<color=green>面部 SDF Keyword 已经强制写入！现在切回材质面板，拉动阈值看有没有效果。</color>");
    }
}
