using UnityEditor;
using UnityEngine;

public class ApiUsageWindow : EditorWindow
{
    [MenuItem("Tools/Unity API Usage Collector")]
    static void Open()
    {
        GetWindow<ApiUsageWindow>("Unity API Usage");
    }

    void OnGUI()
    {
        GUILayout.Label("UnityEngine API Usage Collector", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(5);

        var wasEnabled = ApiUsageInstaller.Enabled;
        ApiUsageInstaller.Enabled =
            EditorGUILayout.Toggle("Enable Collection", ApiUsageInstaller.Enabled);
        
        if (wasEnabled != ApiUsageInstaller.Enabled)
        {
            Debug.Log($"[API Collector] Collection {(ApiUsageInstaller.Enabled ? "enabled" : "disabled")}");
        }

        EditorGUILayout.Space(5);
        
        // 显示收集的数据统计
        var count = ApiUsageRecorder.GetCollectedCount();
        EditorGUILayout.HelpBox($"Collected: {count} unique API calls", MessageType.Info);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Export CSV to Desktop"))
        {
            ApiUsageRecorder.ExportCsv();
        }
        
        EditorGUILayout.Space(5);
        
        if (GUILayout.Button("Clear Data"))
        {
            ApiUsageRecorder.Reset();
            Debug.Log("[API Collector] Data cleared.");
        }
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "1. Enable Collection\n" +
            "2. Enter Play Mode\n" +
            "3. Test your game\n" +
            "4. Exit Play Mode\n" +
            "5. Export CSV", 
            MessageType.None);
    }
}