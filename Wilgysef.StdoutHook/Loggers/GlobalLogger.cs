using System;
using System.IO;

namespace Wilgysef.StdoutHook.Loggers;

public static class GlobalLogger
{
    public static Logger Logger { get; set; } = new Logger(StreamWriter.Null);

    public static string? ProcessName { get; set; }

    public static void Log(LogLevel level, string message) => Logger.Log(level, GetMessage(message));

    public static void Error(string message) => Logger.Error(GetMessage(message));

    public static void Error(Exception exception, string message) => Logger.Error(exception, GetMessage(message));

    public static void Warn(string message) => Logger.Warn(GetMessage(message));

    public static void Info(string message) => Logger.Info(GetMessage(message));

    private static string GetMessage(string message)
    {
        return ProcessName != null
            ? $"{ProcessName}: {message}"
            : message;
    }
}
