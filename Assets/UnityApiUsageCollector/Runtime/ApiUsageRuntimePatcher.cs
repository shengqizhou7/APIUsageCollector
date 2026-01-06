using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 运行时补丁应用器
/// 在场景加载之前应用 Harmony 补丁，确保能抓到所有生命周期方法（Start, Awake, OnApplicationQuit 等）
/// </summary>
public static class ApiUsageRuntimePatcher
{
    private const string EnabledKey = "ApiUsageCollector.Enabled";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
#if UNITY_EDITOR
        // 检查是否启用了收集功能（通过 EditorPrefs）
        bool enabled = EditorPrefs.GetBool(EnabledKey, false);
        
        if (enabled)
        {
            // 清空数据
            ApiUsageRecorder.Reset();
            
            // 应用补丁（在任何 MonoBehaviour 方法执行之前）
            ApiUsageHarmonyPatcher.Patch();
            Debug.Log("[API Collector] Runtime patches applied before scene load.");
        }
#endif
    }
}
