using UnityEngine;
using UnityEditor;

public class RossiMaterialConfigurator
{
    [MenuItem("Tools/SumiToon/Config Rossi Materials (Keywords & Clip)")]
    public static void Configure()
    {
        string matDir = "Assets/ProductAssets/Prefab/Character/Rossi/Materials/";
        
        string[] mainMats = {
            "面部皮肤", "虹膜瞳孔", "眼睛高光", "眼白", "眼睑阴影", 
            "睫毛眉毛", "口腔", "头发", "头发投影", "身体皮肤", 
            "披风白裙与皮革", "金属皮革与硬件", "彩绳饰带", "耳尾毛与蕾丝", "表情图集"
        };

        foreach (var name in mainMats)
        {
            string path = matDir + name + ".mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            // 1. 开启基础裁剪（解决黑色破洞）
            // 头发、睫毛、衣服等需要镂空，但面部、身体不需要
            if (name == "头发" || name == "睫毛眉毛" || name == "眼白" || name == "口腔" || 
                name == "披风白裙与皮革" || name == "耳尾毛与蕾丝" || name == "彩绳饰带" || name == "表情图集")
            {
                mat.SetFloat("_IntClipType", 1); // 1 = Clip
                mat.EnableKeyword("_CLIP_DITHER");
                mat.DisableKeyword("_CLIP_OFF");
            }
            else
            {
                mat.SetFloat("_IntClipType", 0); // 0 = Off
                mat.EnableKeyword("_CLIP_OFF");
                mat.DisableKeyword("_CLIP_DITHER");
            }

            // 2. 开启 Toon 光影特性（让模型立体、有描边）
            mat.EnableKeyword("_OUTLINE_ON");
            mat.SetFloat("_FloatOutlineWidth", 0.005f); // 细描边
            
            mat.EnableKeyword("_RIMLIGHT_ON");
            mat.SetFloat("_FloatRimLightIntensity", 0.5f);

            mat.EnableKeyword("_ADDLIGHT_ON");

            // 3. 面部特殊设置：暂时关闭 SDF，先看正常的色阶表现
            if (name == "面部皮肤")
            {
                mat.DisableKeyword("_FACESDF_ON");
                mat.SetFloat("_ToggleFaceSDF", 0f);
                mat.SetFloat("_ToggleLightHorLockBaseMap", 1f); // 面部锁定水平光
            }

            // 4. 头发的颜色要正确受光
            if (name == "头发")
            {
                mat.EnableKeyword("_HIGHLIGHT_ON");
            }

            EditorUtility.SetDirty(mat);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("<color=green>洛茜全材质关键字配置完成！回到场景看看是不是立体了。</color>");
    }
}
