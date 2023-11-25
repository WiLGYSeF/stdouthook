using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Wilgysef.StdoutHook.Loggers;

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
        var result = builder
            .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
            .Append(": ")
            .Append(GetLogLevel(level))
            .Append(": ")
            .Append(message)
            .ToString();

        Debug.WriteLine(result);
        _stream.WriteLine(result);
    }

    public void Error(string message) => Log(LogLevel.Error, message);

    public void Error(Exception exception, string message)
    {
        Log(LogLevel.Error, message);
        Log(LogLevel.Error, exception.ToString());
    }

    public void Warn(string message) => Log(LogLevel.Warn, message);

    public void Info(string message) => Log(LogLevel.Info, message);

    private static string GetLogLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Error => "ERR ",
            LogLevel.Warn => "WARN",
            LogLevel.Info => "INFO",
            _ => throw new ArgumentOutOfRangeException(nameof(level)),
        };
    }
}
