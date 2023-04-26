using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Cli;

public class StreamOutputHandler
{
    private const int BufferSize = 4096;

    private readonly Profile _profile;
    private readonly StreamReaderHandler _outputReaderHandler;
    private readonly StreamReaderHandler _errorReaderHandler;

    private readonly ProfileState _profileState = new();

    public StreamOutputHandler(Profile profile, StreamReader stdout, StreamReader stderr)
    {
        _profile = profile;
        _outputReaderHandler = new StreamReaderHandler(stdout, HandleOutput);
        _errorReaderHandler = new StreamReaderHandler(stderr, HandleError);
    }

    public async Task ReadLinesAsync(CancellationToken cancellationToken = default)
    {
        var readOutputTask = _outputReaderHandler.ReadLinesAsync(BufferSize, cancellationToken);
        var readErrorTask = _errorReaderHandler.ReadLinesAsync(BufferSize, cancellationToken);

        await Task.WhenAll(readOutputTask, readErrorTask);
    }

    private void HandleOutput(string line)
    {
        _profileState.StdoutLineCount++;

        if (_profile.ApplyRules(line, true, _profileState))
        {
            WriteConsoleOutput(line);
        }
    }

    private void HandleError(string line)
    {
        _profileState.StderrLineCount++;

        if (_profile.ApplyRules(line, false, _profileState))
        {
            WriteConsoleError(line);
        }
    }

    private static void WriteConsoleOutput(string line)
    {
        Console.Write(line);
        //Console.Out.Flush();
    }

    private static void WriteConsoleError(string line)
    {
        Console.Error.Write(line);
        //Console.Error.Flush();
    }
}
