using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class RossiMaterialSetup : EditorWindow
{
    [MenuItem("Tools/SumiToon/Setup Rossi Materials")]
    public static void Setup()
    {
        string charDir = "Assets/ProductAssets/Prefab/Character/Rossi";
        string matDir = charDir + "/Materials";
        
        if (!Directory.Exists(matDir))
            Directory.CreateDirectory(matDir);

        Shader litShader = Shader.Find("SumiToon/Lit");
        if (litShader == null)
        {
            Debug.LogError("找不到 SumiToon/Lit Shader！");
            return;
        }

        Dictionary<string, string> mainMats = new Dictionary<string, string>()
        {
            { "面部皮肤", "Face_D" }, { "虹膜瞳孔", "Iris_D" }, { "眼睛高光", "EyeHighlight_D" },
            { "眼白", "EyeWhite_D" }, { "眼睑阴影", "EyelidShadow_D" }, { "睫毛眉毛", "LashBrow_D" },
            { "口腔", "Mouth_D" }, { "头发", "Hair_D" }, { "头发投影", "HairShadow_D" },
            { "身体皮肤", "BodySkin_D" }, { "披风白裙与皮革", "Cloth_D" }, { "金属皮革与硬件", "Metal_D" },
            { "彩绳饰带", "Ribbon_D" }, { "耳尾毛与蕾丝", "EarTailFur_D" }, { "表情图集", "EmotionAtlas_D" }
        };
        
        Dictionary<string, string> detailMats = new Dictionary<string, string>()
        {
            { "耳内交错短毛｜原色柔绒", "Hair_D" }, { "耳羽蓬松细毛｜原色柔绒", "Hair_D" },
            { "原金色内部晶片", "Iris_D" }, { "尾耳毛附着细丝原色", "Hair_D" }, { "头发附着细丝原色", "Hair_D" }
        };

        Dictionary<string, string> allMats = new Dictionary<string, string>();
        foreach (var kv in mainMats) allMats[kv.Key] = kv.Value;
        foreach (var kv in detailMats) if(!allMats.ContainsKey(kv.Key)) allMats[kv.Key] = kv.Value;

        // 1. 生成并修复材质球
        foreach (var kv in allMats)
        {
            string matName = kv.Key;
            string texName = kv.Value;
            
            string matPath = $"{matDir}/{matName}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                mat = new Material(litShader);
                AssetDatabase.CreateAsset(mat, matPath);
            }
            else
            {
                mat.shader = litShader;
            }

            // 强制绑定贴图
            string texPath = $"{charDir}/{texName}.png";
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            if (tex != null)
            {
                mat.SetTexture("_BaseMap", tex);
            }
            else
            {
                Debug.LogWarning($"找不到贴图: {texPath}");
            }

            if (matName == "面部皮肤")
            {
                mat.EnableKeyword("_FACESDF_ON");
                mat.SetFloat("_ToggleFaceSDF", 1.0f);
                mat.SetFloat("_ToggleLightHorLockBaseMap", 1.0f);
            }
            if (matName == "虹膜瞳孔" || matName == "原金色内部晶片")
            {
                mat.EnableKeyword("_MATCAP_ON");
                mat.SetFloat("_ToggleMatCap", 1.0f);
            }

            EditorUtility.SetDirty(mat);
        }

        AssetDatabase.SaveAssets();

        // 2. 自动挂载到所有 FBX
        AssignMaterialsToFBX(charDir + "/Rossi_rig.fbx", matDir, mainMats);
        AssignMaterialsToFBX(charDir + "/Rossi_EarFurInner_skin.fbx", matDir, detailMats);
        AssignMaterialsToFBX(charDir + "/Rossi_EarFurOuter_skin.fbx", matDir, detailMats);
        AssignMaterialsToFBX(charDir + "/Rossi_IrisCrystal_skin.fbx", matDir, detailMats);
        AssignMaterialsToFBX(charDir + "/Rossi_FurStrand13_skin.fbx", matDir, detailMats);
        AssignMaterialsToFBX(charDir + "/Rossi_FurStrand07_skin.fbx", matDir, detailMats);

        AssetDatabase.Refresh();
        Debug.Log("洛茜材质修复与自动绑定完成！请检查场景里的模型。如果还是灰色，请从 Project 里把 Rossi_rig.fbx 重新拖一次进场景。");
    }

    private static void AssignMaterialsToFBX(string fbxPath, string matDir, Dictionary<string, string> matMap)
    {
        ModelImporter importer = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (importer == null) return;

        // 修复：确保 Material Creation Mode 是 Standard，这样 Remap 才能生效
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
        importer.materialLocation = ModelImporterMaterialLocation.External;

        AssetImporter.SourceAssetIdentifier[] ids = importer.GetExternalObjectMap().Keys.ToArray();
        foreach (var id in ids) importer.RemoveRemap(id);

        foreach (var kv in matMap)
        {
            string matName = kv.Key;
            string matPath = $"{matDir}/{matName}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat != null)
            {
                importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), matName), mat);
            }
        }
        
        importer.SaveAndReimport();
    }
}
