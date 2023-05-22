namespace Wilgysef.StdoutHook.Cli;

internal class ProgramSetupException : Exception
{
    public int ExitCode { get; }

    public ProgramSetupException() : this(1, "", null) { }

    public ProgramSetupException(string message) : this(1, message, null) { }

    public ProgramSetupException(string message, Exception? innerException) : this(1, message, innerException) { }

    public ProgramSetupException(int exitCode, string message) : this(exitCode, message, null) { }

    public ProgramSetupException(int exitCode, string message, Exception? innerException) : base(message, innerException)
    {
        ExitCode = exitCode;
    }
}
