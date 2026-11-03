using UnityEngine; 
using UnityEditor; 

public class PerfectWaterSetup 
{ 
    [MenuItem("Tools/SumiToon/Setup Perfect Water")] 
    public static void Setup() {
        string matDir = "Assets/ProductAssets/Water";
        if (!AssetDatabase.IsValidFolder(matDir)) {
            AssetDatabase.CreateFolder("Assets/ProductAssets", "Water");
        }

        string matPath = matDir + "/StylizedWaterMat.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat == null) { 
            mat = new Material(Shader.Find("Custom/StylizedWater")); 
            AssetDatabase.CreateAsset(mat, matPath); 
        }
        
        // 自动挂法线图
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ProductAssets/SharedResources/NormalMap/NormalMap_WaterDistort.png");
        if (tex != null) mat.SetTexture("_NormalMap", tex);
        
        // 强制写入你截图里完美的参数
        mat.SetColor("_ShallowColor", new Color(0.1f, 0.8f, 0.9f, 0.2f)); // 薄荷青，低不透明度
        mat.SetColor("_DeepColor", new Color(0.05f, 0.2f, 0.5f, 1f)); // 深海蓝
        mat.SetFloat("_DepthMaxDistance", 1.5f);
        
        mat.SetColor("_SpecColor", new Color(1f, 1f, 1f, 1f));
        mat.SetFloat("_SpecThreshold", 0.8f);
        mat.SetFloat("_SpecSmoothness", 0.5f);
        
        mat.SetColor("_FoamColor", new Color(1f, 1f, 1f, 1f));
        mat.SetFloat("_FoamDistance", 0.15f);
        mat.SetFloat("_FoamThreshold", 0.8f);
        
        mat.SetFloat("_RefractionIntensity", 0.03f);
        mat.SetFloat("_NormalScale", 0.5f);
        mat.SetFloat("_WaveSpeed", 0.5f);

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();

        // 检查或创建水面
        GameObject plane = GameObject.Find("WaterPlane");
        if (plane == null) { 
            plane = GameObject.CreatePrimitive(PrimitiveType.Plane); 
            plane.name = "WaterPlane"; 
            plane.transform.position = new Vector3(0, 0, 0); 
            plane.transform.localScale = new Vector3(2, 2, 2); 
        }
        plane.GetComponent<MeshRenderer>().sharedMaterial = mat;

        Debug.Log("<color=green>完美水体已配置完成！请去场景查看。</color>");
    } 
}