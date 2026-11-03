using UnityEngine; 
using UnityEditor; 

public class FixWaterClarity 
{ 
    [MenuItem("Tools/SumiToon/Fix Water Clarity")] 
    public static void Fix() {
        string matPath = "Assets/ProductAssets/Water/StylizedWaterMat.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        
        if (mat == null) {
            Debug.LogError("找不到材质，请先运行 Setup Perfect Water");
            return;
        }

        // 1. 强制把浅水颜色拉低，Alpha 设为极低（几乎全透明）
        mat.SetColor("_ShallowColor", new Color(0.6f, 0.9f, 0.9f, 0.0f)); 
        
        // 2. 深水颜色保持蓝色，但也让它稍微透一点
        mat.SetColor("_DeepColor", new Color(0.1f, 0.4f, 0.7f, 0.8f));
        
        // 3. 【核心关键】拉大深度衰减距离。水越浅，透得越彻底。
        // 如果你觉得还不够透，可以继续加大这个值（比如加到 5.0）
        mat.SetFloat("_DepthMaxDistance", 3.0f); 
        
        // 4. 泡沫收缩，只贴在最边上
        mat.SetFloat("_FoamDistance", 0.1f);
        mat.SetFloat("_FoamThreshold", 0.85f);

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        
        Debug.Log("<color=green>水面清透度参数已重置！如果还看不见底，请检查 URP Asset 里的 Opaque Texture 是否勾选。</color>");
    } 
}