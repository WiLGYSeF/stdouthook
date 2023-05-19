using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Cli;

public class StreamOutputHandler
{
    private readonly Profile _profile;
    private readonly StreamReaderHandler _outputReaderHandler;
    private readonly StreamReaderHandler _errorReaderHandler;
    private readonly TextWriter _stdout;
    private readonly TextWriter _stderr;

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
    }

    public async Task ReadLinesAsync(int bufferSize = 4096, CancellationToken cancellationToken = default)
    {
        var readOutputTask = _outputReaderHandler.ReadLinesAsync(bufferSize, cancellationToken);
        var readErrorTask = _errorReaderHandler.ReadLinesAsync(bufferSize, cancellationToken);

        await Task.WhenAll(readOutputTask, readErrorTask);
    }

    private void HandleOutput(string line)
    {
        _profile.State.StdoutLineCount++;

        var output = _profile.ApplyRules(line, true);
        if (output != null)
        {
            _stdout.Write(output);
        }
    }

    private void HandleError(string line)
    {
        _profile.State.StderrLineCount++;

        var output = _profile.ApplyRules(line, false);
        if (output != null)
        {
            _stderr.Write(output);
        }
    }
}
