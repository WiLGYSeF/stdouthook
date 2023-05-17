using System;
using System.IO;
using System.Text;

namespace Wilgysef.StdoutHook.Loggers
{
    public class Logger : ILogger
    {
        private readonly StreamWriter _stream;

        public Logger(StreamWriter stream)
        {
            _stream = stream;
        }

        public void Log(LogLevel level, string message)
        {
            var builder = new StringBuilder();
            _stream.WriteLine(builder
                .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
                .Append(": ")
                .Append(GetLogLevel(level))
                .Append(": ")
                .Append(message)
                .ToString());
        }

        public void Error(string message) => Log(LogLevel.Error, message);

        public void Warn(string message) => Log(LogLevel.Warn, message);

        public void Info(string message) => Log(LogLevel.Info, message);

        private static string GetLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Error:
                    return "ERR ";
                case LogLevel.Warn:
                    return "WARN";
                case LogLevel.Info:
                    return "INFO";
                default:
                    throw new ArgumentOutOfRangeException(nameof(level));
            }
        }
    }
}
