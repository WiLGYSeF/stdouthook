using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Cli;

public class StreamOutputHandler : IDisposable
{
    private const int BufferSize = 4096;

    public bool FlushOutput { get; set; }

    public bool FlushError { get; set; }

    private readonly Profile _profile;
    private readonly StreamReaderHandler _outputReaderHandler;
    private readonly StreamReaderHandler _errorReaderHandler;

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

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private void HandleOutput(string line)
    {
        _profile.State.StdoutLineCount++;

        if (_profile.ApplyRules(ref line, true))
        {
            WriteConsoleOutput(line);
        }
    }

    private void HandleError(string line)
    {
        _profile.State.StderrLineCount++;

        if (_profile.ApplyRules(ref line, false))
        {
            WriteConsoleError(line);
        }
    }

    private void WriteConsoleOutput(string line)
    {
        Console.Write(line);

        if (FlushOutput)
        {
            Console.Out.Flush();
        }
    }

    private void WriteConsoleError(string line)
    {
        Console.Error.Write(line);

        if (FlushError)
        {
            Console.Error.Flush();
        }
    }
}
