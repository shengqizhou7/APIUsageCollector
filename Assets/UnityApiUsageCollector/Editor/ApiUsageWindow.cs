using UnityEditor;
using UnityEngine;

public class ApiUsageWindow : EditorWindow
{
    [MenuItem("Tools/API Usage Collector")]
    static void Open()
    {
        GetWindow<ApiUsageWindow>("Unity API Usage");
    }

    void OnGUI()
    {
        var titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleLeft
        };
        
        GUILayout.Label("UnityEngine API Usage Collector", titleStyle);
        
        EditorGUILayout.Space(10);

        var wasEnabled = ApiUsageInstaller.Enabled;
        ApiUsageInstaller.Enabled =
            EditorGUILayout.Toggle("Enable Collection", ApiUsageInstaller.Enabled);
        
        if (wasEnabled != ApiUsageInstaller.Enabled)
        {
            Debug.Log($"[API Collector] Collection {(ApiUsageInstaller.Enabled ? "enabled" : "disabled")}");
        }

        EditorGUILayout.Space(5);
        
        var count = ApiUsageRecorder.GetCollectedCount();
        EditorGUILayout.BeginHorizontal();
        
        var helpBoxRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(30));
        EditorGUI.HelpBox(helpBoxRect, $"Collected: {count} unique API calls", MessageType.Info);
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Clear Data", GUILayout.Width(100), GUILayout.Height(30)))
        {
            ApiUsageRecorder.Reset();
            Debug.Log("[API Collector] Data cleared.");
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        if (GUILayout.Button("Export CSV to Desktop", GUILayout.Height(38)))
        {
            ApiUsageRecorder.ExportCsv();
        }
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Steps:\n" + 
            "1. Enable Collection\n" +
            "2. Enter Play Mode\n" +
            "3. Test your game (Clear collected data if necessary)\n" +
            "4. Exit Play Mode\n" +
            "5. Export CSV",
            MessageType.None);
    }
}