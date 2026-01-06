using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[InitializeOnLoad]
public static class ApiUsageInstaller
{
    const string EnabledKey = "ApiUsageCollector.Enabled";
    
    public static bool Enabled
    {
        get => EditorPrefs.GetBool(EnabledKey, false);
        set => EditorPrefs.SetBool(EnabledKey, value);
    }

    static ApiUsageInstaller()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (!Enabled) return;

        // RuntimeInitializeOnLoadMethod 会在 BeforeSceneLoad 时自动应用补丁
        // 这里只需要处理退出和清理
        
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // RuntimeInitializeOnLoadMethod 已经应用了补丁
            Debug.Log("[API Collector] Play mode entered, collecting data...");
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // 退出前显示统计，但保持 Patch 以便抓到 OnApplicationQuit
            var count = ApiUsageRecorder.GetCollectedCount();
            Debug.Log($"[API Collector] Collected {count} unique API calls. Ready to export.");
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            // 完全退出 Play 模式后再移除补丁
            ApiUsageHarmonyPatcher.Unpatch();
            Debug.Log("[API Collector] Exited play mode, patches removed.");
        }
    }
}