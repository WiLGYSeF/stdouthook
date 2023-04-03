// TODO: use https://github.com/microsoft/vs-pty.net

using System.Diagnostics;
using Wilgysef.StdoutHook.ActionCommands;
using Wilgysef.StdoutHook.Cli;

var processInfo = new ProcessStartInfo("python")
{
    RedirectStandardError = true,
    RedirectStandardOutput = true,
};

processInfo.ArgumentList.Add("D:\\projects\\stdouthook\\Wilgysef.StdoutHook\\testproc.py");

//var processInfo = new ProcessStartInfo("pip")
//{
//    RedirectStandardError = true,
//    RedirectStandardOutput = true,
//};

//processInfo.ArgumentList.Add("install");
//processInfo.ArgumentList.Add("yt-dlp");

var stopwatch = Stopwatch.StartNew();
var process = Process.Start(processInfo);

var threadRun = true;
var errorReceived = 0;
var outputReceived = 0;
var largestDiff = 0;

var outputCommands = new List<ActionCommand>()
{
    new TestActionCommand(OutputHandler),
};
var errorCommands = new List<ActionCommand>()
{
    new TestActionCommand(ErrorHandler),
};

var commandHandler = new ActionCommandHandler(outputCommands, errorCommands);

//process.OutputDataReceived += commandHandler.HandleOutput;
//process.ErrorDataReceived += commandHandler.HandleError;
//process.BeginOutputReadLine();
//process.BeginErrorReadLine();

var outputReaderHandler = new StreamReaderHandler(process.StandardOutput, commandHandler.HandleOutput);
var errorReaderHandler = new StreamReaderHandler(process.StandardError, commandHandler.HandleError);

var readOutputTask = outputReaderHandler.ReadLinesAsync(bufferSize: 4096);
var readErrorTask = errorReaderHandler.ReadLinesAsync(bufferSize: 4096);

//var readOutputThread = new Thread(() => ReadStreamReaderThread(process.StandardOutput, OutputHandler));
//var readErrorThread = new Thread(() => ReadStreamReaderThread(process.StandardError, ErrorHandler));
//readOutputThread.Start();
//readErrorThread.Start();

//process.WaitForExit();
Console.ReadLine();

stopwatch.Stop();
threadRun = false;
process.Kill();

Console.WriteLine($"done out: {outputReceived} err: {errorReceived} diff: {largestDiff} elapsed: {stopwatch.ElapsedMilliseconds} ({(double)stopwatch.ElapsedMilliseconds / Math.Max(outputReceived, errorReceived)})");

string OutputHandler(string line)
{
    outputReceived++;

    if (outputReceived - errorReceived > largestDiff)
    {
        largestDiff = outputReceived - errorReceived;
    }

    return $"{outputReceived} {line}";
}

string ErrorHandler(string line)
{
    errorReceived++;

    if (errorReceived - outputReceived > largestDiff)
    {
        largestDiff = errorReceived - outputReceived;
    }

    return $"{errorReceived} {line}";
}

void ReadStreamReaderThread(StreamReader reader, Action<string> action)
{
    while (!reader.EndOfStream && threadRun)
    {
        var line = reader.ReadLine();
        if (line != null)
        {
            action(line);
        }
    }
}
