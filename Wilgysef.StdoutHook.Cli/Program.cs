using CommandLine;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wilgysef.StdoutHook.Cli;
using Wilgysef.StdoutHook.Loggers;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;

const string LogName = ".stdouthook.log";
const int ProcessRefreshInterval = 1000;
const int BufferSizeDefault = 16384;
const int OutputFlushIntervalDefault = 250;
const int InteractiveFlushIntervalDefault = 200;

StreamWriter? logStream = null;
Profile? profile = null;
Process? process = null;
TextWriter? outputStreamWriter = null;
TextWriter? errorStreamWriter = null;
StreamOutputHandler? streamOutputHandler = null;

var argparseTimeElapsed = TimeSpan.Zero;
var profileLoadingTimeElapsed = TimeSpan.Zero;
var processRuntime = TimeSpan.Zero;

var globalStopwatch = Stopwatch.StartNew();
var processStopwatch = new Stopwatch();
var stopwatch = new Stopwatch();

try
{
    stopwatch.Restart();
    var argParseResult = new Parser(parser =>
    {
        parser.AllowMultiInstance = true;
        parser.EnableDashDash = true;
        parser.HelpWriter = Console.Error;
    })
        .ParseArguments<Options>(args);
    argparseTimeElapsed = stopwatch.Elapsed;

    if (argParseResult.Tag == ParserResultType.NotParsed)
    {
        throw new ProgramSetupException();
    }

    Shared.Options = argParseResult.Value;
    ValidateArgs();

    var configDir = GetConfigurationDirectory(Shared.Options.ConfigDir);

    if (Shared.Options.ColorDebug)
    {
        ColorDebug.WriteColorDebug(Console.Out);
        return 0;
    }

    if (Shared.Options.Arguments.Count == 0)
    {
        Console.Error.WriteLine("ERROR: command is required");
        return 1;
    }

    logStream = new StreamWriter(GetLogStream(
        configDir != null
            ? Path.Combine(configDir, LogName)
            : null));
    GlobalLogger.Logger = new Logger(logStream);

    stopwatch.Restart();

    var command = Shared.Options.Arguments[0];
    var commandArguments = Shared.Options.Arguments.Skip(1).ToArray();

    var commandPaths = new CommandLocator().LocateCommand(command);
    var fullCommandPath = commandPaths.FirstOrDefault() ?? command;

    var cliProfileDtoLoader = new CliProfileDtoLoader();
    var profileDtos = configDir != null
        ? await cliProfileDtoLoader.LoadProfileDtosAsync(configDir)
        : new List<ProfileDto>();

    var profileLoaded = false;
    try
    {
        (profile, var profileDtoPicked) = LoadProfile(
            profileDtos,
            Shared.Options.ProfileName,
            command,
            fullCommandPath,
            commandArguments);
        profileLoaded = profile != null;

        Shared.VerbosePrint(profileLoaded
            ? $"using profile: {profile!.ProfileName ?? "<unnamed>"}"
            : "no profile selected");

        if (profileLoaded && Shared.Options.Verbose >= 2)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
            };
            Shared.VerbosePrint($"using profile: {JsonSerializer.Serialize(profileDtoPicked, options)}", 2);
        }
    }
    catch (Exception ex)
    {
        Shared.VerbosePrintError($"failed to load profile: {ex.GetType().Name} {ex.Message}");
        GlobalLogger.Error(ex, "failed to load profile");
    }

    try
    {
        profile?.Build();
    }
    catch (Exception ex)
    {
        Shared.VerbosePrintError($"failed to build profile: {ex.Message}");
        GlobalLogger.Error(ex, "failed to build profile");
        profileLoaded = false;
    }

    profileLoadingTimeElapsed = stopwatch.Elapsed;

    if (Shared.Options.Stdout != null)
    {
        outputStreamWriter = CreateRedirectedStream(Shared.Options.Stdout, Shared.Options.StdoutAppend);
    }

    if (Shared.Options.Stderr != null)
    {
        errorStreamWriter = CreateRedirectedStream(Shared.Options.Stderr, Shared.Options.StderrAppend);
    }

    processStopwatch.Start();
    process = StartProcess(profile, fullCommandPath, commandArguments);

    try
    {
        GlobalLogger.ProcessName = process.ProcessName;
    }
    catch { }

    Task? readStreamTask = null;
    using var cancellationTokenSource = new CancellationTokenSource();

    if (profileLoaded)
    {
        profile!.State.SetProcess(process);

        var flush = profile.Flush || Shared.Options.Flush;
        var bufferSize = flush ? 1 : (Shared.Options.BufferSize ?? profile.BufferSize ?? BufferSizeDefault);
        outputStreamWriter ??= new CustomStreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding, bufferSize);
        errorStreamWriter ??= new CustomStreamWriter(Console.OpenStandardError(), Console.OutputEncoding, bufferSize);

        profile.Split(out var stdoutProfile, out var stderrProfile);

        streamOutputHandler = new StreamOutputHandler(
            stdoutProfile,
            stderrProfile,
            process.StandardOutput,
            process.StandardError,
            outputStreamWriter,
            errorStreamWriter);

        readStreamTask = streamOutputHandler.ReadLinesAsync(
            forceProcessTimeout: (Shared.Options.Interactive || profile.Interactive)
                ? TimeSpan.FromMilliseconds(Shared.Options.InteractiveFlushInterval
                    ?? profile.InteractiveFlushInterval
                    ?? InteractiveFlushIntervalDefault)
                : null,
            cancellationToken: cancellationTokenSource.Token);

        if (!flush)
        {
            _ = Task.Run(async () =>
            {
                //periodically flush the output

                var cancellationToken = cancellationTokenSource.Token;
                var interval = Shared.Options.OutputFlushInterval ?? profile.OutputFlushInterval ?? OutputFlushIntervalDefault;

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Delay(interval, cancellationToken);

                    outputStreamWriter.Flush();
                    errorStreamWriter.Flush();
                }
            });
        }

        _ = Task.Run(async () =>
        {
            var cancellationToken = cancellationTokenSource.Token;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(ProcessRefreshInterval, cancellationToken);

                process.Refresh();
            }
        });
    }

    try
    {
        process.WaitForExit();
    }
    catch { }

    cancellationTokenSource.Cancel();

    if (readStreamTask != null)
    {
        // log any exception that may have occurred

        try
        {
            await readStreamTask;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            GlobalLogger.Error(ex, "exception occurred");
        }
    }

    try
    {
        return process.HasExited
            ? process.ExitCode
            : 0;
    }
    catch
    {
        return 0;
    }
}
catch (ProgramSetupException setupException)
{
    Shared.ErrorEx(setupException.InnerException, setupException.Message);
    return setupException.ExitCode;
}
catch (Exception ex)
{
    Shared.VerbosePrintError("uncaught exception");
    Shared.VerbosePrintError(ex.ToString());
    return 1;
}
finally
{
    processStopwatch.Stop();
    processRuntime = processStopwatch.Elapsed;

    if (process != null)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill();
            }
        }
        catch { }

        process.Dispose();
    }

    try
    {
        streamOutputHandler?.Dispose();
    }
    catch { }

    outputStreamWriter?.Dispose();
    errorStreamWriter?.Dispose();

    // TODO: remove
    if (Shared.Options.Verbose >= 3)
    {
        Shared.VerbosePrint("", 3);
        Shared.VerbosePrint($"argument parsing: {argparseTimeElapsed}", 3);
        Shared.VerbosePrint($"profile loading:  {profileLoadingTimeElapsed}", 3);
        Shared.VerbosePrint($"process runtime:  {processRuntime}", 3);
    }

    logStream?.Dispose();
}

(Profile?, ProfileDto?) LoadProfile(
    IReadOnlyList<ProfileDto> profileDtos,
    string? profileName,
    string command,
    string? fullCommandPath,
    IReadOnlyList<string> arguments)
{
    var loader = new ProfileLoader();
    var picker = new ProfileDtoPicker();

    ProfileDto? profileDtoPicked = null;
    var profile = loader.LoadProfile(
        profileDtos,
        profiles =>
        {
            profileDtoPicked = picker.PickProfileDto(
                profiles,
                profileName: profileName,
                command: Path.GetFileName(command),
                fullCommandPath: fullCommandPath,
                arguments: arguments);
            return profileDtoPicked;
        },
        throwIfInheritedProfileNotFound: false);

    return (profile, profileDtoPicked);
}

Process StartProcess(Profile? profile, string command, IReadOnlyList<string> arguments)
{
    try
    {
        var process = Process.Start(CreateProcessStartInfo(profile, command, arguments));
        if (process == null)
        {
            throw new ProgramSetupException($"process could not be started: {command}");
        }

        return process;
    }
    catch (Exception ex)
    {
        throw new ProgramSetupException($"process could not be started: {ex.Message}: {command}", ex);
    }
}

ProcessStartInfo CreateProcessStartInfo(Profile? profile, string command, IReadOnlyList<string> arguments)
{
    var redirect = profile != null;
    var processInfo = new ProcessStartInfo(command)
    {
        RedirectStandardError = redirect,
        RedirectStandardOutput = redirect,
    };

    for (var i = 0; i < arguments.Count; i++)
    {
        processInfo.ArgumentList.Add(arguments[i]);
    }

    return processInfo;
}

TextWriter CreateRedirectedStream(string filename, bool append)
{
    try
    {
        var dirname = Path.GetDirectoryName(filename);
        if (dirname != null && dirname.Length > 0)
        {
            Directory.CreateDirectory(dirname);
        }

        return new StreamWriter(new FileStream(
            filename,
            append ? FileMode.Append : FileMode.Open,
            FileAccess.Write));
    }
    catch (Exception ex)
    {
        throw new ProgramSetupException($"could not open file: {ex.Message}: {filename}");
    }
}

Stream GetLogStream(string? filename)
{
    if (filename == null)
    {
        Shared.VerbosePrintError("ERROR: failed to open log file");
        return new MemoryStream();
    }

    try
    {
        return new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.Read);
    }
    catch (Exception ex)
    {
        Shared.VerbosePrintError($"ERROR: failed to open log file: {ex.Message}");
        return new MemoryStream();
    }
}

string? GetConfigurationDirectory(string? configDir)
{
    if (configDir == null)
    {
        var home = Environment.GetEnvironmentVariable("HOME");
        configDir = home != null
            ? Path.Combine(home, ".stdouthook")
            : null;
    }

    if (configDir != null)
    {
        try
        {
            Directory.CreateDirectory(configDir);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ERROR: could not create config directory: {configDir}: {ex.Message}");
            return null;
        }
    }

    return configDir;
}

void ValidateArgs()
{
    if (Shared.Options.BufferSize.HasValue && Shared.Options.BufferSize.Value < 1)
    {
        Shared.Options.BufferSize = null;
    }

    if (Shared.Options.OutputFlushInterval.HasValue && Shared.Options.OutputFlushInterval.Value < 0)
    {
        Shared.Options.OutputFlushInterval = null;
    }

    if (Shared.Options.InteractiveFlushInterval.HasValue && Shared.Options.InteractiveFlushInterval.Value < 0)
    {
        Shared.Options.InteractiveFlushInterval = null;
    }
}
