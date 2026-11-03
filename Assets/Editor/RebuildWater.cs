using UnityEngine; 
using UnityEditor; 

public class RebuildWater { 
    [MenuItem("Tools/SumiToon/Rebuild Water Plane")] 
    public static void Rebuild() {
        string matPath = "Assets/ProductAssets/Water/StylizedWaterMat.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat == null) { 
            mat = new Material(Shader.Find("Custom/StylizedWater")); 
            AssetDatabase.CreateAsset(mat, matPath); 
        }
        // 自动挂法线图
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ProductAssets/SharedResources/NormalMap/NormalMap_WaterDistort.png");
        if (tex != null) mat.SetTexture("_NormalMap", tex);
        
        GameObject plane = GameObject.Find("WaterPlane");
        if (plane == null) { 
            plane = GameObject.CreatePrimitive(PrimitiveType.Plane); 
            plane.name = "WaterPlane"; 
            plane.transform.position = new Vector3(0, 0, 0); 
            plane.transform.localScale = new Vector3(2, 2, 2); 
        }
        plane.GetComponent<MeshRenderer>().sharedMaterial = mat;
        Debug.Log("水面已重建完毕！");
    } 
}