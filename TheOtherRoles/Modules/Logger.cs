using BepInEx;
using BepInEx.Logging;
using System.Diagnostics;
using System.Text;

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
        StackFrame stack = new(2);
        var time = DateTime.Now.ToString("HH:mm:ss");
        var className = stack.GetMethod()?.ReflectedType?.Name;
        className = string.IsNullOrWhiteSpace(className) ? "" : $" [{className}]";
        tag = string.IsNullOrWhiteSpace(tag) ? "" : $" [{tag}]";
        text = $"[{time}]{className}{tag} {text}";

        switch (logLevel)
        {
            case LogLevel.Message:
                logSource.LogMessage(text);
                break;
            case LogLevel.Error:
                logSource.LogError(text);
                break;
            case LogLevel.Warning:
                logSource.LogWarning(text);
                break;
            case LogLevel.Fatal:
                logSource.LogFatal(text);
                break;
            case LogLevel.Info:
                logSource.LogInfo(text);
                break;
            case LogLevel.Debug:
                logSource.LogDebug(text);
                break;
            default:
                logSource.LogInfo(text);
                break;
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
                throw new ArgumentOutOfRangeException(nameof(errorLevel), errorLevel, null);
        }
    }

    public static void LogObject(object @object)
    {
        FastLog(LogLevel.Error, @object);
    }
}