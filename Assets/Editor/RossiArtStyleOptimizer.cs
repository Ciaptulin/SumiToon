using UnityEngine;
using UnityEditor;

public class RossiArtStyleOptimizer
{
    [MenuItem("Tools/SumiToon/Optimize Rossi Art Style")]
    public static void Optimize()
    {
        string matDir = "Assets/ProductAssets/Prefab/Character/Rossi/Materials/";
        string[] allMats = {
            "面部皮肤", "虹膜瞳孔", "眼睛高光", "眼白", "眼睑阴影", 
            "睫毛眉毛", "口腔", "头发", "头发投影", "身体皮肤", 
            "披风白裙与皮革", "金属皮革与硬件", "彩绳饰带", "耳尾毛与蕾丝", "表情图集"
        };

        foreach (var name in allMats)
        {
            string path = matDir + name + ".mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            // --- 1. 瘦身描边 ---
            if (name == "面部皮肤" || name == "眼白" || name == "眼睑阴影" || name == "口腔")
            {
                // 脸部和眼睛内部不需要黑线描边
                mat.SetFloat("_FloatOutlineWidth", 0f); 
            }
            else
            {
                // 其他部位描边变细，并开启基础色混合（让线带有衣服原本的颜色）
                mat.SetFloat("_FloatOutlineWidth", 0.0015f); 
                mat.SetFloat("_ToggleOutlineBaseMapBlend", 1f);
                // 描边颜色不要死黑，用深灰
                mat.SetColor("_ColorOutlineColor", new Color(0.15f, 0.15f, 0.15f, 1f));
            }

            // --- 2. 柔化暗部 ---
            // 默认的暗部可能太硬，把色阶过渡拉软一点
            mat.SetFloat("_FloatBrightShade1Blur", 0.2f);
            mat.SetFloat("_FloatShade1Shade2Blur", 0.5f);

            // --- 3. 头发/衣物增强质感 ---
            if (name == "头发")
            {
                mat.EnableKeyword("_HIGHLIGHT_ON");
                mat.SetFloat("_ToggleHighLight", 1f);
                mat.SetFloat("_FloatHighLightSize", 0.4f); // 天使环大小
                mat.SetFloat("_ToggleHighLightSoft", 1f);  // 柔和高光
                // 高光颜色调成浅金色
                mat.SetColor("_ColorHighLightColor", new Color(1f, 0.95f, 0.8f, 1f));
            }
            
            if (name == "金属皮革与硬件")
            {
                mat.EnableKeyword("_HIGHLIGHT_ON");
                mat.SetFloat("_ToggleHighLight", 1f);
                mat.SetFloat("_FloatHighLightSize", 0.2f);
                // 金属高光偏白
                mat.SetColor("_ColorHighLightColor", new Color(0.8f, 0.8f, 0.8f, 1f));
            }

            EditorUtility.SetDirty(mat);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("<color=green>美术优化完成！描边变细，头发明暗高光已开启。</color>");
    }
}
