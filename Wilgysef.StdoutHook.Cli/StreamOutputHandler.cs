using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Cli;

public class StreamOutputHandler
{
    private const int BufferSize = 4096;

    public bool FlushOutput
    {
        get => _flushOutput;
        set
        {
            _flushOutput = value;
            _writeOutput = _flushOutput ? WriteOutputFlush : WriteOutput;
        }
    }

    public bool FlushError
    {
        get => _flushError;
        set
        {
            _flushError = value;
            _writeError = _flushError ? WriteErrorFlush : WriteError;
        }
    }

    private readonly Profile _profile;
    private readonly StreamReaderHandler _outputReaderHandler;
    private readonly StreamReaderHandler _errorReaderHandler;
    private readonly TextWriter _stdout;
    private readonly TextWriter _stderr;

    private Action<string> _writeOutput;
    private Action<string> _writeError;

    private bool _flushOutput;
    private bool _flushError;

    public StreamOutputHandler(
        Profile profile,
        StreamReader stdoutInput,
        StreamReader stderrInput,
        TextWriter stdoutOutput,
        TextWriter stderrOutput)
    {
        _profile = profile;
        _outputReaderHandler = new StreamReaderHandler(stdoutInput, HandleOutput);
        _errorReaderHandler = new StreamReaderHandler(stderrInput, HandleError);
        _stdout = stdoutOutput;
        _stderr = stderrOutput;

        _writeOutput = WriteOutput;
        _writeError = WriteError;
    }

    public async Task ReadLinesAsync(CancellationToken cancellationToken = default)
    {
        var readOutputTask = _outputReaderHandler.ReadLinesAsync(BufferSize, cancellationToken);
        var readErrorTask = _errorReaderHandler.ReadLinesAsync(BufferSize, cancellationToken);

        await Task.WhenAll(readOutputTask, readErrorTask);
    }

    private void HandleOutput(string line)
    {
        _profile.State.StdoutLineCount++;

        if (_profile.ApplyRules(ref line, true))
        {
            _writeOutput(line);
        }
    }

    private void HandleError(string line)
    {
        _profile.State.StderrLineCount++;

        if (_profile.ApplyRules(ref line, false))
        {
            _writeError(line);
        }
    }

    private void WriteOutput(string line)
    {
        _stdout.Write(line);
    }

    private void WriteOutputFlush(string line)
    {
        _stdout.Write(line);
        _stdout.Flush();
    }

    private void WriteError(string line)
    {
        _stderr.Write(line);
    }

    private void WriteErrorFlush(string line)
    {
        _stderr.Write(line);
        _stderr.Flush();
    }
}
