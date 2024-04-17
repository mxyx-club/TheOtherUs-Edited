using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using InnerNet;

namespace TheOtherRoles.Helper;

internal static class LogHelper
{
    private static ManualLogSource logSource { get; set; }

    internal static void SetLogSource(ManualLogSource Source)
    {
        if (ConsoleManager.ConsoleEnabled) System.Console.OutputEncoding = Encoding.UTF8;
        logSource = Source;
    }

    /// <summary>
    ///     一般信息
    /// </summary>
    /// <param name="Message"></param>
    public static void Info(string Message)
    {
        logSource.LogInfo(Message);
    }

    /// <summary>
    ///     报错
    /// </summary>
    /// <param name="Message"></param>
    public static void Error(string Message)
    {
        logSource.LogError(Message);
    }

    /// <summary>
    ///     测试
    /// </summary>
    /// <param name="Message"></param>
    public static void Debug(string Message)
    {
        logSource.LogDebug(Message);
    }

    public static void Fatal(string Message)
    {
        logSource.LogFatal(Message);
    }

    /// <summary>
    ///     警告
    /// </summary>
    /// <param name="Message"></param>
    public static void Warn(string Message)
    {
        logSource.LogWarning(Message);
    }


    public static void Message(string Message)
    {
        logSource.LogMessage(Message);
    }

    public static void Exception(Exception exception)
    {
        Error(exception.ToString());
    }

    public static void FastLog(LogLevel errorLevel, object @object)
    {
        var Logger = logSource;
        var Message = @object as string;
        switch (errorLevel)
        {
            case LogLevel.Message:
                Logger.LogMessage(Message);
                break;
            case LogLevel.Error:
                Logger.LogError(Message);
                break;
            case LogLevel.Warning:
                Logger.LogWarning(Message);
                break;
            case LogLevel.Fatal:
                Logger.LogFatal(Message);
                break;
            case LogLevel.Info:
                Logger.LogInfo(Message);
                break;
            case LogLevel.Debug:
                Logger.LogDebug(Message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(errorLevel), errorLevel, null);
        }
    }

    public static void LogObject(object @object)
    {
        FastLog(LogLevel.Error, @object);
    }
}

#if DEBUG
[HarmonyPatch]
internal static class LogListener
{
    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> taregetMethodBases() => typeof(AmongUsClient).Assembly.GetTypes()
        .Where(n => n.IsSubclassOf(typeof(InnerNetObject)))
        .Select(x => x.GetMethod(nameof(InnerNetObject.HandleRpc), AccessTools.allDeclared))
        .Where(m => m != null);
    
    [HarmonyPostfix]
    internal static void OnRpc(InnerNetObject __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] Hazel.MessageReader reader)
    {
        Info($"{__instance.name} {callId} {reader.Length} {reader.Tag}");
    }
}
#endif