// TODO: use https://github.com/microsoft/vs-pty.net

using System.Diagnostics;
using Wilgysef.StdoutHook.Cli;
using Wilgysef.StdoutHook.CommandLocator;
using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;

var command = "python";
var commandPaths = new CommandLocator().LocateCommand(command);

var fullCommandPath = commandPaths.FirstOrDefault();
var arguments = new[]
{
    "D:\\projects\\stdouthook\\Wilgysef.StdoutHook\\testproc.py",
};

var loader = new JsonProfileLoader();
using var stream = File.Open("test.json", FileMode.Open);
using var stream1 = File.Open("test1.json", FileMode.Open);

var dtos = new List<ProfileDto>
{
    await loader.LoadProfileDtoAsync(stream),
    await loader.LoadProfileDtoAsync(stream1),
};

var picker = new ProfileDtoPicker();
using var profile = loader.LoadProfile(
    dtos,
    profileDtos => picker.PickProfileDto(
        profileDtos,
        profileName: "test",
        command: command,
        fullCommandPath: fullCommandPath,
        arguments: arguments));

//ColorDebug.GetColorDebug(Console.Out);
//return;

var processInfo = new ProcessStartInfo(command)
{
    RedirectStandardError = true,
    RedirectStandardOutput = true,
};

for (var i = 0; i < arguments.Length; i++)
{
    processInfo.ArgumentList.Add(arguments[i]);
}

//var processInfo = new ProcessStartInfo("pip")
//{
//    RedirectStandardError = true,
//    RedirectStandardOutput = true,
//};

//processInfo.ArgumentList.Add("install");
//processInfo.ArgumentList.Add("yt-dlp");

profile.Build();

var stopwatch = Stopwatch.StartNew();
var process = Process.Start(processInfo);

if (process == null)
{
    throw new Exception("Process could not start.");
}

profile.State.SetProcess(process);

var streamOutputHandler = new StreamOutputHandler(
    profile,
    process.StandardOutput,
    process.StandardError,
    Console.Out,
    Console.Error);
streamOutputHandler.FlushOutput = profile.Flush;
streamOutputHandler.FlushError = profile.Flush;

var cancellationTokenSource = new CancellationTokenSource();
var readStreamTask = streamOutputHandler.ReadLinesAsync(cancellationTokenSource.Token);

//process.WaitForExit();
Console.ReadLine();

stopwatch.Stop();
cancellationTokenSource.Cancel();
process.Kill();
