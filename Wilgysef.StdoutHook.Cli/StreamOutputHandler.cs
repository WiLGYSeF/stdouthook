using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Cli;

public class StreamOutputHandler : IDisposable
{
    private readonly Profile _stdoutProfile;
    private readonly Profile _stderrProfile;
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
        : this(profile, profile, stdoutInput, stderrInput, stdoutOutput, stderrOutput) { }

    public StreamOutputHandler(
        Profile stdoutProfile,
        Profile stderrProfile,
        StreamReader stdoutInput,
        StreamReader stderrInput,
        TextWriter stdoutOutput,
        TextWriter stderrOutput)
    {
        _stdoutProfile = stdoutProfile;
        _stderrProfile = stderrProfile;
        _outputReaderHandler = new StreamReaderHandler(stdoutInput, HandleOutput);
        _errorReaderHandler = new StreamReaderHandler(stderrInput, HandleError);
        _stdout = stdoutOutput;
        _stderr = stderrOutput;
    }

    public async Task ReadLinesAsync(
        int bufferSize = 4096,
        TimeSpan? forceProcessTimeout = null,
        CancellationToken cancellationToken = default)
    {
        var readOutputTask = _outputReaderHandler.ReadLinesAsync(bufferSize, forceProcessTimeout, cancellationToken);
        var readErrorTask = _errorReaderHandler.ReadLinesAsync(bufferSize, forceProcessTimeout, cancellationToken);

        await Task.WhenAll(readOutputTask, readErrorTask);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _outputReaderHandler.Dispose();
        _errorReaderHandler.Dispose();
    }

    private void HandleOutput(string line)
    {
        _stdoutProfile.State.StdoutLineCount++;

        var output = _stdoutProfile.ApplyRules(line, true);
        if (output != null)
        {
            _stdout.Write(output);
        }
    }

    private void HandleError(string line)
    {
        _stderrProfile.State.StderrLineCount++;

        var output = _stderrProfile.ApplyRules(line, false);
        if (output != null)
        {
            _stderr.Write(output);
        }
    }
}
