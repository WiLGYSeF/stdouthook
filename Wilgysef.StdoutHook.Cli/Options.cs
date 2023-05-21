using CommandLine;

namespace Wilgysef.StdoutHook.Cli;

internal class Options
{
    [Option("profile", MetaValue = "NAME", HelpText = "Profile name to use for command")]
    public string? ProfileName { get; set; }

    [Option("flush", HelpText = "Flush output")]
    public bool Flush { get; set; }

    [Option("buffer-size", Default = 16384, HelpText = "Output buffer size (default 16384)")]
    public int BufferSize { get; set; }

    [Option("stdout", MetaValue = "FILE", HelpText = "Redirect stdout to file")]
    public string? Stdout { get; set; }

    [Option("stdout-append", HelpText = "Append to redirected stdout file instead of creating a new file")]
    public bool StdoutAppend { get; set; }

    [Option("stderr", MetaValue = "FILE", HelpText = "Redirect stderr to file")]
    public string? Stderr { get; set; }

    [Option("stderr-append", HelpText = "Append to redirected stderr file instead of creating a new file")]
    public bool StderrAppend { get; set; }

    [Option('v', "verbose", FlagCounter = true, HelpText = "Verbose level")]
    public int Verbose { get; set; }

    [Option("color-debug", HelpText = "Show the color debug output and exit")]
    public bool ColorDebug { get; set; }

    [Option("config", MetaValue = "DIR", HelpText = "Use a custom configuration directory path")]
    public string? ConfigDir { get; set; }

    [Value(0)]
    public IList<string> Arguments { get; set; } = Array.Empty<string>();
}
