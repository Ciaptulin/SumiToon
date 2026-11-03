using UnityEngine;
using UnityEditor;

public class RossiSceneFixer
{
    [MenuItem("Tools/SumiToon/Fix Rossi Scene Materials")]
    public static void FixMaterials()
    {
        // 找到场景里所有的 SkinnedMeshRenderer 和 MeshRenderer
        var renderers = Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int fixedCount = 0;

        string matDir = "Assets/ProductAssets/Prefab/Character/Rossi/Materials/";

        foreach (var r in renderers)
        {
            // 只处理洛茜相关的模型
            if (!r.name.Contains("洛茜") && !r.name.Contains("Rossi")) continue;

            var mats = r.sharedMaterials;
            bool changed = false;

            for (int i = 0; i < mats.Length; i++)
            {
                string targetMatName = "";

                // 根据网格和槽位索引匹配材质
                if (r.name.Contains("洛茜_mesh") || r.name.Contains("Rossi.002"))
                {
                    // 主网格的 15 个槽位
                    string[] mainMats = {
                        "面部皮肤", "虹膜瞳孔", "眼睛高光", "眼白", "眼睑阴影", 
                        "睫毛眉毛", "口腔", "头发", "头发投影", "身体皮肤", 
                        "披风白裙与皮革", "金属皮革与硬件", "彩绳饰带", "耳尾毛与蕾丝", "表情图集"
                    };
                    if (i < mainMats.Length) targetMatName = mainMats[i];
                }
                else if (r.name.Contains("金属平面") || r.name.Contains("Rossi.004"))
                {
                    // 金属倒角副本（15个槽位，但只用到了金属）
                    if (i == 11) targetMatName = "金属皮革与硬件";
                }
                else if (r.name.Contains("耳内交错短毛") || r.name.Contains("EarFurInner"))
                {
                    targetMatName = "耳内交错短毛｜原色柔绒";
                }
                else if (r.name.Contains("耳羽蓬松细毛") || r.name.Contains("EarFurOuter"))
                {
                    targetMatName = "耳羽蓬松细毛｜原色柔绒";
                }
                else if (r.name.Contains("虹膜内部晶片") || r.name.Contains("IrisCrystal"))
                {
                    targetMatName = "原金色内部晶片";
                }
                else if (r.name.Contains("附着细丝｜13") || r.name.Contains("FurStrand13"))
                {
                    targetMatName = "尾耳毛附着细丝原色";
                }
                else if (r.name.Contains("附着细丝｜7") || r.name.Contains("FurStrand07"))
                {
                    targetMatName = "头发附着细丝原色";
                }

                if (!string.IsNullOrEmpty(targetMatName))
                {
                    Material targetMat = AssetDatabase.LoadAssetAtPath<Material>(matDir + targetMatName + ".mat");
                    if (targetMat != null && mats[i] != targetMat)
                    {
                        mats[i] = targetMat;
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                Undo.RecordObject(r, "Fix Rossi Materials");
                r.sharedMaterials = mats;
                fixedCount++;
                Debug.Log($"已修复 {r.name} 的材质。");
            }
        }

        if (fixedCount > 0)
            Debug.Log($"<color=green>场景修复完成！共修复了 {fixedCount} 个渲染器的材质。</color>");
        else
            Debug.LogWarning("没有找到需要修复的洛茜材质，请确认模型在场景里。");
    }
}
