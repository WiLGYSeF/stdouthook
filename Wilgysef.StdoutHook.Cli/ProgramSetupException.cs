namespace Wilgysef.StdoutHook.Cli;

internal class ProgramSetupException : Exception
{
    public ProgramSetupException()
        : this(1, "", null)
    {
    }

    public ProgramSetupException(string message, Exception? innerException = null)
        : this(1, message, innerException)
    {
    }

    public ProgramSetupException(int exitCode, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        ExitCode = exitCode;
    }

    public int ExitCode { get; }
}
