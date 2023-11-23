using System.Diagnostics;
using Wilgysef.StdoutHook.Loggers;

namespace Wilgysef.StdoutHook.Cli;

internal static class Shared
{
    public static Options Options = null!;

    public static void Error(string message)
    {
        Console.Error.WriteLine("STDOUTHOOK: " + message);
        GlobalLogger.Error(message);
    }

    public static void ErrorEx(Exception? ex, string message, string? consoleMessage = null)
    {
        Console.Error.WriteLine("STDOUTHOOK: " + (consoleMessage ?? message));

        if (ex != null)
        {
            GlobalLogger.Error(ex, message);
        }
    }

    public static void VerbosePrint(string message, int level = 1)
    {
        var formattedMessage = "STDOUTHOOK: " + message;
        Debug.WriteLine(formattedMessage);

        if (Options != null && Options.Verbose >= level)
        {
            Console.WriteLine(formattedMessage);
        }
    }

    public static void VerbosePrintError(string message, int level = 1)
    {
        var formattedMessage = "STDOUTHOOK: " + message;
        Debug.WriteLine(formattedMessage);

        if (Options != null && Options.Verbose >= level)
        {
            Console.Error.WriteLine(formattedMessage);
        }
    }

}
