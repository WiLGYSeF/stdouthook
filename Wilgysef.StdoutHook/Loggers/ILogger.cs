using System;

namespace Wilgysef.StdoutHook.Loggers
{
    public interface ILogger
    {
        void Log(LogLevel level, string message);

        void Error(string message);

        void Error(Exception exception, string message);

        void Warn(string message);

        void Info(string message);
    }
}
