#if UNITY_EDITOR
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
#endif

using UnityEngine;

/// <summary>
/// Harmony 补丁管理器
/// 使用条件编译确保只在 Editor 模式下编译 Harmony 相关代码
/// </summary>
public static class ApiUsageHarmonyPatcher
{
#if UNITY_EDITOR
    static Harmony harmony;
    static bool patched;

    public static void Patch()
    {
        if (patched) return;

        harmony = new Harmony("unity.api.usage.collector");

        int patchedCount = 0;
        int typeCount = 0;
        
        var asm = Assembly.Load("Assembly-CSharp");
        foreach (var type in asm.GetTypes())
        {
            if (!IsUserType(type)) continue;
            typeCount++;

            foreach (var method in type.GetMethods(
                         BindingFlags.Instance | BindingFlags.Static |
                         BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!IsSafeMethod(method)) continue;

                try
                {
                    harmony.Patch(
                        method,
                        transpiler: new HarmonyMethod(
                            typeof(ApiUsageHarmonyPatcher),
                            nameof(Transpiler)
                        )
                    );
                    patchedCount++;
                }
                catch {}
            }
        }

        Debug.Log($"[API Collector] Patched {patchedCount} methods in {typeCount} user types.");
        patched = true;
    }

    public static void Unpatch()
    {
        if (!patched) return;
        harmony.UnpatchAll("unity.api.usage.collector");
        patched = false;
    }
    
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        // 获取 Record 方法，使用新的签名：Record(RuntimeMethodHandle, RuntimeTypeHandle)
        var recordMethod = typeof(ApiUsageRecorder).GetMethod(
            nameof(ApiUsageRecorder.Record), 
            BindingFlags.Public | BindingFlags.Static,
            null,
            new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) },
            null);

        if (recordMethod == null)
        {
            Debug.LogError("[API Collector] Failed to find Record method with correct signature (RuntimeMethodHandle, RuntimeTypeHandle)!");
            foreach (var ins in instructions)
                yield return ins;
            yield break;
        }

        foreach (var ins in instructions)
        {
            // 先输出原指令
            yield return ins;

            // 如果是 UnityEngine API 调用，插入记录代码
            if (ins.opcode == OpCodes.Call || ins.opcode == OpCodes.Callvirt)
            {
                if (ins.operand is MethodInfo mi &&
                    mi.DeclaringType?.Namespace?.StartsWith("UnityEngine") == true)
                {
                    // 插入: ApiUsageRecorder.Record(mi.MethodHandle, mi.DeclaringType.TypeHandle);
                    yield return new CodeInstruction(OpCodes.Ldtoken, mi);
                    yield return new CodeInstruction(OpCodes.Ldtoken, mi.DeclaringType);
                    yield return new CodeInstruction(OpCodes.Call, recordMethod);
                }
            }
        }
    }

    // 判断是否是用户定义的类型
    static bool IsUserType(Type type)
    {
        if (type == null) return false;
        
        // 排除编译器生成的类型
        if (type.Name.Contains("<") || type.Name.Contains(">")) 
            return false;
        
        // 排除特殊类型
        if (type.IsSpecialName || type.IsInterface || type.IsEnum)
            return false;
            
        return true;
    }

    // 判断方法是否可以安全地被 patch
    static bool IsSafeMethod(MethodInfo method)
    {
        if (method == null) return false;
        
        // 排除抽象方法
        if (method.IsAbstract) return false;
        
        // 排除泛型方法定义
        if (method.IsGenericMethodDefinition) return false;
        
        // 排除编译器生成的方法
        if (method.Name.Contains("<") || method.Name.Contains(">"))
            return false;
        
        // 排除特殊方法（属性访问器等）
        if (method.IsSpecialName) return false;
        
        return true;
    }
#else
    // 非 Editor 模式下提供空实现
    public static void Patch()
    {
        Debug.LogWarning("[API Collector] Patch is only available in Unity Editor.");
    }

    public static void Unpatch()
    {
        Debug.LogWarning("[API Collector] Unpatch is only available in Unity Editor.");
    }
#endif
}
