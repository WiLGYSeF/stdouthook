// TODO: use https://github.com/microsoft/vs-pty.net

using System.Diagnostics;
using Wilgysef.StdoutHook.Cli;
using Wilgysef.StdoutHook.CommandLocator;
using Wilgysef.StdoutHook.Loggers;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;

//ColorDebug.GetColorDebug(Console.Out);
//return;

using var logStream = new StreamWriter(GetLogStream("log.txt"));
GlobalLogger.Logger = new Logger(logStream);

var profileName = "test";
var command = "python";
var commandPaths = new CommandLocator().LocateCommand(command);
var fullCommandPath = commandPaths.FirstOrDefault();
var arguments = new[]
{
    "D:\\projects\\stdouthook\\Wilgysef.StdoutHook\\testproc.py",
};

var loader = new JsonProfileLoader();
var extensions = new HashSet<string> { "", ".json", ".txt", ".yaml", ".yml" };
var profileDtos = await LoadProfilesFromDirectoryAsync(loader, ".", extensions);

var picker = new ProfileDtoPicker();
using var profile = loader.LoadProfile(
    profileDtos,
    profiles => picker.PickProfileDto(
        profiles,
        profileName: profileName,
        command: command,
        fullCommandPath: fullCommandPath,
        arguments: arguments),
    throwIfInheritedProfileNotFound: false);
var profileLoaded = profile != null;

try
{
    profile?.Build();
}
catch (Exception ex)
{
    GlobalLogger.Error(ex, "failed to build profile");
    profileLoaded = false;
}

Process? process;

try
{
    process = Process.Start(CreateProcessStartInfo(profile, command, arguments));
    if (process == null)
    {
        Console.Error.WriteLine($"process could not be started: {command}");
        GlobalLogger.Error($"process could not be started: {command}");
        return 1;
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"process could not be started: {ex.Message}: {command}");
    GlobalLogger.Error(ex, $"process could not be started: {command}");
    return 1;
}

using var __process = process;
Task? readStreamTask = null;
var cancellationTokenSource = new CancellationTokenSource();

if (profileLoaded)
{
    profile!.State.SetProcess(process);

    var bufferSize = profile.Flush ? 1 : 16384;
    var outputStreamWriter = new CustomStreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding, bufferSize);
    var errorStreamWriter = new CustomStreamWriter(Console.OpenStandardError(), Console.OutputEncoding, bufferSize);

    var streamOutputHandler = new StreamOutputHandler(
        profile,
        process.StandardOutput,
        process.StandardError,
        outputStreamWriter,
        errorStreamWriter);

    readStreamTask = streamOutputHandler.ReadLinesAsync(cancellationToken: cancellationTokenSource.Token);

    if (!profile.Flush)
    {
        _ = Task.Run(async () =>
        {
            //periodically flush the output

            var cancellationToken = cancellationTokenSource.Token;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(250, cancellationToken);

                outputStreamWriter.Flush();
                errorStreamWriter.Flush();
            }
        });
    }
}

_ = Task.Run(async () =>
{
    var cancellationToken = cancellationTokenSource.Token;
    while (true)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.Delay(1000, cancellationToken);

        process.Refresh();
    }
});

// TODO: remove test code
Console.ReadLine();
process.Kill();

process.WaitForExit();
cancellationTokenSource.Cancel();

if (readStreamTask != null)
{
    try
    {
        await readStreamTask;
    }
    catch (Exception ex)
    {
        GlobalLogger.Error(ex, "exception occurred");
    }
}

return process.HasExited
    ? process.ExitCode
    : 0;

ProcessStartInfo CreateProcessStartInfo(Profile? profile, string command, string[] arguments)
{
    var redirect = profile != null;

    var processInfo = new ProcessStartInfo(command)
    {
        RedirectStandardError = redirect,
        RedirectStandardOutput = redirect,
    };

    for (var i = 0; i < arguments.Length; i++)
    {
        processInfo.ArgumentList.Add(arguments[i]);
    }

    return processInfo;
}

async Task<List<ProfileDto>> LoadProfilesFromDirectoryAsync(ProfileLoader loader, string path, IReadOnlyCollection<string> extensions)
{
    var profiles = new List<ProfileDto>();
    var files = Directory.GetFiles(path);

    for (var i = 0; i < files.Length; i++)
    {
        var file = files[i];
        var extension = Path.GetExtension(file);

        if (!extensions.Contains(extension))
        {
            continue;
        }

        try
        {
            using var stream = File.Open(file, FileMode.Open, FileAccess.Read);
            profiles.AddRange(await loader.LoadProfileDtosAsync(stream));
        }
        catch (Exception ex)
        {
            GlobalLogger.Error($"failed to load profiles from '{file}': {ex.Message}");
        }
    }

    return profiles;
}

Stream GetLogStream(string filename)
{
    try
    {
        return new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.Read);
    }
    catch
    {
        Debug.WriteLine("ERROR: failed to open log file");
        return new MemoryStream();
    }
}
