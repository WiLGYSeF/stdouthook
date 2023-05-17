using System;
using System.IO;

namespace Wilgysef.StdoutHook.Loggers
{
    public static class GlobalLogger
    {
        public static Logger Logger { get; set; } = new Logger(new StreamWriter(new MemoryStream()));

        public static void Log(LogLevel level, string message) => Logger.Log(level, message);

        public static void Error(string message) => Logger.Error(message);

        public static void Error(Exception exception, string message) => Logger.Error(exception, message);

        public static void Warn(string message) => Logger.Warn(message);

        public static void Info(string message) => Logger.Info(message);
    }
}
