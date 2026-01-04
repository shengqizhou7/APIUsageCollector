using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public static class ApiUsageRecorder
{
    static Dictionary<RuntimeMethodHandle, int> counts = new();
    static Dictionary<RuntimeMethodHandle, string> methodNames = new();
    [ThreadStatic] static bool inRecord;

    public static void Record(RuntimeMethodHandle handle)
    {
        if (inRecord) return;
        inRecord = true;

        try
        {
            counts.TryGetValue(handle, out int c);
            counts[handle] = c + 1;

            // 缓存方法名
            if (!methodNames.ContainsKey(handle))
            {
                try
                {
                    var method = MethodBase.GetMethodFromHandle(handle);
                    methodNames[handle] = $"{method.DeclaringType?.FullName}.{method.Name}";
                }
                catch
                {
                    methodNames[handle] = "Unknown";
                }
            }
        }
        finally
        {
            inRecord = false;
        }
    }

    public static void Reset()
    {
        counts.Clear();
        methodNames.Clear();
    }

    public static int GetCollectedCount()
    {
        return counts.Count;
    }

    public static void ExportCsv()
    {
        if (counts.Count == 0)
        {
            UnityEngine.Debug.LogWarning("[API Collector] No data collected. Make sure you enabled collection and ran Play mode.");
            return;
        }

        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "unity_api_usage.csv");

        using var sw = new StreamWriter(path);
        sw.WriteLine("Method,Count");

        // 按调用次数降序排序
        foreach (var kv in counts.OrderByDescending(x => x.Value))
        {
            var name = methodNames.TryGetValue(kv.Key, out var n) ? n : "Unknown";
            sw.WriteLine($"\"{name}\",{kv.Value}");
        }

        UnityEngine.Debug.Log($"[API Collector] Exported {counts.Count} unique API calls to: {path}");
    }
}