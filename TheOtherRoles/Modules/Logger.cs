using System.Diagnostics;
using System.Text;
using BepInEx;
using BepInEx.Logging;

namespace TheOtherRoles.Modules;

internal static class Logger
{
    private static ManualLogSource logSource { get; set; }

    internal static void SetLogSource(ManualLogSource Source)
    {
        if (ConsoleManager.ConsoleEnabled) System.Console.OutputEncoding = Encoding.UTF8;
        logSource = Source;
    }

    public static void Info(object text, string Tag = "") => SendLog(text.ToString(), Tag, LogLevel.Info);
    public static void Message(object text, string Tag = "") => SendLog(text.ToString(), Tag, LogLevel.Message);
    public static void Warn(object text, string Tag = "") => SendLog(text.ToString(), Tag, LogLevel.Warning);
    public static void Error(object text, string Tag = "") => SendLog(text.ToString(), Tag, LogLevel.Error);
    public static void Debug(object text, string Tag = "") => SendLog(text.ToString(), Tag, LogLevel.Debug);
    public static void Fatal(object text, string Tag = "") => SendLog(text.ToString(), Tag, LogLevel.Fatal);

    public static void SendLog(string text, string tag = "", LogLevel logLevel = LogLevel.Info)
    {
        if (logSource == null) return;

        var time = DateTime.Now.ToString("HH:mm:ss");
        var prefix = string.IsNullOrEmpty(tag)
            ? new StackTrace(2, false).GetFrame(0)?.GetMethod()?.Name is string name ? $"[{time}] [{name}]" : $"[{time}]"
            : $"[{time}] [{tag}]";
        var logMessage = $"{prefix} {text}";

        switch (logLevel)
        {
            case LogLevel.Message: logSource.LogMessage(logMessage); break;
            case LogLevel.Error: logSource.LogError(logMessage); break;
            case LogLevel.Warning: logSource.LogWarning(logMessage); break;
            case LogLevel.Fatal: logSource.LogFatal(logMessage); break;
            case LogLevel.Info: logSource.LogInfo(logMessage); break;
            case LogLevel.Debug: logSource.LogDebug(logMessage); break;
            default: System.Console.WriteLine($"[Error] {logMessage}"); break;
        }
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
                System.Console.WriteLine($"[Error] {Message}");
                break;
        }
    }

    public static void LogObject(object @object)
    {
        FastLog(LogLevel.Error, @object);
    }
}