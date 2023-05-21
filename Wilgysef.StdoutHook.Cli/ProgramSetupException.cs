namespace Wilgysef.StdoutHook.Cli;

internal class ProgramSetupException : Exception
{
    public int ExitCode { get; }

    public ProgramSetupException() : this(1, "") { }

    public ProgramSetupException(string message) : this(1, message) { }

    public ProgramSetupException(int exitCode, string message) : base(message)
    {
        ExitCode = exitCode;
    }
}
