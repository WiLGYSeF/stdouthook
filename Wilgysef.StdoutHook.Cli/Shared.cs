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

    public static void ErrorEx(Exception ex, string message, string? consoleMessage = null)
    {
        Console.Error.WriteLine("STDOUTHOOK: " + (consoleMessage ?? message));
        GlobalLogger.Error(ex, message);
    }

    public static void VerbosePrint(string message)
    {
        var formattedMessage = "STDOUTHOOK: " + message;
        Debug.WriteLine(formattedMessage);

        if (Options != null && Options.Verbose > 0)
        {
            Console.WriteLine(formattedMessage);
        }
    }

    public static void VerbosePrintError(string message)
    {
        var formattedMessage = "STDOUTHOOK: " + message;
        Debug.WriteLine(formattedMessage);

        if (Options != null && Options.Verbose > 0)
        {
            Console.Error.WriteLine(formattedMessage);
        }
    }

}
