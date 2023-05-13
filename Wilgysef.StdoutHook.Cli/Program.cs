// TODO: use https://github.com/microsoft/vs-pty.net

using System.Diagnostics;
using Wilgysef.StdoutHook.Cli;
using Wilgysef.StdoutHook.Profiles;

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

using var profileState = new ProfileState();
var profile = new Profile(profileState);

profile.Build();

var stopwatch = Stopwatch.StartNew();
var process = Process.Start(processInfo);

if (process == null)
{
    throw new Exception("Process could not start.");
}

profileState.SetProcess(process);

using var streamOutputHandler = new StreamOutputHandler(profile, process.StandardOutput, process.StandardError);
var cancellationTokenSource = new CancellationTokenSource();
var readStreamTask = streamOutputHandler.ReadLinesAsync(cancellationTokenSource.Token);

//process.WaitForExit();
Console.ReadLine();

stopwatch.Stop();
cancellationTokenSource.Cancel();
process.Kill();
