using UnityEngine;
using UnityEditor;

public class CleanMissingScripts
{
    [MenuItem("Tools/SumiToon/Clean Missing Scripts in Scene")]
    public static void Clean()
    {
        int count = 0;
        // 使用新版 API 搜索全场景物体
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject go in allObjects)
        {
            int originalCount = go.GetComponents<Component>().Length;
            // 移除所有 Missing 的脚本
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            int newCount = go.GetComponents<Component>().Length;
            
            if (originalCount != newCount)
            {
                count++;
                Debug.Log($"清理了 {go.name} 上的丢失脚本");
            }
        }
        Debug.Log($"<color=green>清理完成！共清理了 {count} 个带有丢失脚本的物体。</color>");
    }
}
