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

        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // 在进入Play模式之前，先清空数据
            ApiUsageRecorder.Reset();
            Debug.Log("[API Collector] Data reset, ready to collect.");
        }
        else if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // 进入Play模式后立即应用补丁
            ApiUsageHarmonyPatcher.Patch();
            Debug.Log("[API Collector] Harmony patches applied.");
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // 退出Play模式前，显示收集的数据统计
            var count = ApiUsageRecorder.GetCollectedCount();
            Debug.Log($"[API Collector] Collected {count} unique API calls. Ready to export.");
            
            ApiUsageHarmonyPatcher.Unpatch();
        }
    }
}