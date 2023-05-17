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

using var logStream = GetLogStream("log.txt");
GlobalLogger.Logger = new Logger(new StreamWriter(logStream));

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

using var _ = process;

if (profileLoaded)
{
    profile!.State.SetProcess(process);

    var streamOutputHandler = new StreamOutputHandler(
        profile,
        process.StandardOutput,
        process.StandardError,
        Console.Out,
        Console.Error)
    {
        FlushOutput = profile.Flush,
        FlushError = profile.Flush,
    };

    var cancellationTokenSource = new CancellationTokenSource();
    var readStreamTask = streamOutputHandler.ReadLinesAsync(cancellationTokenSource.Token);

    // TODO: remove test code
    Console.ReadLine();
    process.Kill();

    cancellationTokenSource.Cancel();
}
else
{
    // TODO: remove test code
    Console.ReadLine();
    process.Kill();
}

process.WaitForExit();

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
        return new FileStream(filename, FileMode.Append);
    }
    catch
    {
        Debug.WriteLine("ERROR: failed to open log file");
        return new MemoryStream();
    }
}
