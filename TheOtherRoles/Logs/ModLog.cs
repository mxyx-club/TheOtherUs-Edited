using System;
using BepInEx.Logging;

namespace TheOtherRoles.Logs;

internal static class ModLog
{
    internal static ManualLogSource logSource { get; set; }

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
        }
    }

    public static void LogObject(object @object)
    {
        FastLog(LogLevel.Error, @object);
    }
}