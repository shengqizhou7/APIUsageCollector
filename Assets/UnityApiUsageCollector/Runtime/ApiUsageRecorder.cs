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

    public static void Record(RuntimeMethodHandle methodHandle, RuntimeTypeHandle typeHandle)
    {
        if (inRecord) return;
        inRecord = true;

        try
        {
            counts.TryGetValue(methodHandle, out int c);
            counts[methodHandle] = c + 1;

            // 缓存方法名
            if (!methodNames.ContainsKey(methodHandle))
            {
                try
                {
                    // 使用带类型句柄的重载，可以正确解析泛型方法
                    var method = MethodBase.GetMethodFromHandle(methodHandle, typeHandle);
                    
                    if (method != null)
                    {
                        // 使用简洁的类型名称格式
                        string typeName = FormatTypeName(method.DeclaringType);
                        string fullName = $"{typeName}.{method.Name}";
                        
                        // 添加参数信息
                        try
                        {
                            var parameters = method.GetParameters();
                            if (parameters.Length > 0)
                            {
                                var paramStr = string.Join(", ", parameters.Select(p => FormatTypeName(p.ParameterType)));
                                fullName += $"({paramStr})";
                            }
                            else
                            {
                                fullName += "()";
                            }
                        }
                        catch { }
                        
                        methodNames[methodHandle] = fullName;
                    }
                    else
                    {
                        methodNames[methodHandle] = "Unknown (null method)";
                        UnityEngine.Debug.LogWarning($"[API Collector] Failed to get method from handle: method is null");
                    }
                }
                catch (Exception ex)
                {
                    methodNames[methodHandle] = $"Unknown ({ex.GetType().Name})";
                    UnityEngine.Debug.LogWarning($"[API Collector] Failed to resolve method: {ex.Message}");
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

    /// <summary>
    /// 格式化类型名称，去除程序集信息，简化泛型显示
    /// </summary>
    static string FormatTypeName(Type type)
    {
        if (type == null) return "Unknown";
        
        // 处理泛型类型
        if (type.IsGenericType)
        {
            // 获取不带 `1 后缀的类型名
            var genericTypeName = type.Name;
            int backtickIndex = genericTypeName.IndexOf('`');
            if (backtickIndex > 0)
            {
                genericTypeName = genericTypeName.Substring(0, backtickIndex);
            }
            
            // 获取泛型参数
            var genericArgs = type.GetGenericArguments();
            var argNames = string.Join(", ", genericArgs.Select(t => t.Name));
            
            return $"{type.Namespace}.{genericTypeName}<{argNames}>";
        }
        
        // 普通类型
        return type.FullName ?? type.Name;
    }
}