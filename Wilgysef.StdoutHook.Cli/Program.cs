using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommandLine;
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

Options options = null!;
StreamWriter? logStream = null;
Profile? profile = null;
Process? process = null;
TextWriter? outputStreamWriter = null;
TextWriter? errorStreamWriter = null;
StreamOutputHandler? streamOutputHandler = null;

var argparseTimeElapsed = TimeSpan.Zero;
var profileLoadingTimeElapsed = TimeSpan.Zero;
var processRuntime = TimeSpan.Zero;

// TODO: remove
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

    options = argParseResult.Value;
    ValidateArgs();

    var configDir = GetConfigurationDirectory(options.ConfigDir);

    if (options.ColorDebug)
    {
        ColorDebug.WriteColorDebug(Console.Out);
        return 0;
    }

    if (options.Arguments.Count == 0)
    {
        throw new ProgramSetupException("command is required");
    }

    logStream = new StreamWriter(GetLogStream(
        configDir != null
            ? Path.Combine(configDir, LogName)
            : null));
    GlobalLogger.Logger = new Logger(logStream);

    stopwatch.Restart();

    var command = options.Arguments[0];
    var commandArguments = options.Arguments.Skip(1).ToArray();

    var commandPaths = new CommandLocator()
        .LocateCommand(command);
    var fullCommandPath = commandPaths.FirstOrDefault()
        ?? command;

    var cliProfileDtoLoader = new CliProfileDtoLoader(VerbosePrint);
    var profileDtos = configDir != null
        ? await cliProfileDtoLoader.LoadProfileDtosAsync(configDir)
        : new List<ProfileDto>();

    var profileLoaded = false;
    try
    {
        (profile, var profileDtoPicked) = LoadProfile(
            profileDtos,
            options.ProfileName,
            command,
            fullCommandPath,
            commandArguments);
        profileLoaded = profile != null;

        VerbosePrint(profileLoaded
            ? $"using profile: {profile!.ProfileName ?? "<unnamed>"}"
            : "no profile selected");

        if (profileLoaded && options.Verbose >= 2)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
            };
            VerbosePrint($"using profile: {JsonSerializer.Serialize(profileDtoPicked, jsonOptions)}", 2);
        }
    }
    catch (Exception ex)
    {
        VerbosePrintError($"failed to load profile: {ex.GetType().Name} {ex.Message}");
        GlobalLogger.Error(ex, "failed to load profile");
    }

    try
    {
        profile?.Build();
    }
    catch (Exception ex)
    {
        VerbosePrintError($"failed to build profile: {ex.Message}");
        GlobalLogger.Error(ex, "failed to build profile");
        profileLoaded = false;
    }

    profileLoadingTimeElapsed = stopwatch.Elapsed;

    if (options.Stdout != null)
    {
        outputStreamWriter = CreateRedirectedStream(options.Stdout, options.StdoutAppend);
    }

    if (options.Stderr != null)
    {
        errorStreamWriter = CreateRedirectedStream(options.Stderr, options.StderrAppend);
    }

    processStopwatch.Start();
    process = StartProcess(profile, fullCommandPath, commandArguments);

    try
    {
        GlobalLogger.ProcessName = process.ProcessName;
    }
    catch
    {
    }

    Task? readStreamTask = null;
    using var cancellationTokenSource = new CancellationTokenSource();

    if (profileLoaded)
    {
        profile!.State.SetProcess(process);

        var flush = profile.Flush || options.Flush;
        var bufferSize = flush
            ? 1
            : (options.BufferSize ?? profile.BufferSize ?? BufferSizeDefault);
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
            forceProcessTimeout: (options.Interactive || profile.Interactive)
                ? TimeSpan.FromMilliseconds(options.InteractiveFlushInterval
                    ?? profile.InteractiveFlushInterval
                    ?? InteractiveFlushIntervalDefault)
                : null,
            cancellationToken: cancellationTokenSource.Token);

        if (!flush)
        {
            _ = Task.Run(async () =>
            {
                // periodically flush the output

                var cancellationToken = cancellationTokenSource.Token;
                var interval = options.OutputFlushInterval
                    ?? profile.OutputFlushInterval
                    ?? OutputFlushIntervalDefault;

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
    catch
    {
    }

    cancellationTokenSource.Cancel();

    if (readStreamTask != null)
    {
        // log any exception that may have occurred

        try
        {
            await readStreamTask;
        }
        catch (OperationCanceledException)
        {
        }
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
    Error(setupException.InnerException, setupException.Message);
    return setupException.ExitCode;
}
catch (Exception ex)
{
    VerbosePrintError("uncaught exception");
    VerbosePrintError(ex.ToString());
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
        catch
        {
        }

        process.Dispose();
    }

    try
    {
        streamOutputHandler?.Dispose();
    }
    catch
    {
    }

    outputStreamWriter?.Dispose();
    errorStreamWriter?.Dispose();

    // TODO: remove
    if (options != null && options.Verbose >= 3)
    {
        VerbosePrint("", 3);
        VerbosePrint($"argument parsing: {argparseTimeElapsed}", 3);
        VerbosePrint($"profile loading:  {profileLoadingTimeElapsed}", 3);
        VerbosePrint($"process runtime:  {processRuntime}", 3);
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
        return Process.Start(CreateProcessStartInfo(profile, command, arguments))
            ?? throw new ProgramSetupException($"process could not be started: {command}");
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
            append ? FileMode.Append : FileMode.OpenOrCreate,
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
        VerbosePrintError("ERROR: failed to open log file");
        return new MemoryStream();
    }

    try
    {
        return new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.Read);
    }
    catch (Exception ex)
    {
        VerbosePrintError($"ERROR: failed to open log file: {ex.Message}");
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
    if (options.BufferSize.HasValue && options.BufferSize.Value < 1)
    {
        options.BufferSize = null;
    }

    if (options.OutputFlushInterval.HasValue && options.OutputFlushInterval.Value < 0)
    {
        options.OutputFlushInterval = null;
    }

    if (options.InteractiveFlushInterval.HasValue && options.InteractiveFlushInterval.Value < 0)
    {
        options.InteractiveFlushInterval = null;
    }
}

void Error(Exception? ex, string message, string? consoleMessage = null)
{
    Console.Error.WriteLine("STDOUTHOOK: " + (consoleMessage ?? message));

    if (ex != null)
    {
        GlobalLogger.Error(ex, message);
    }
}

void VerbosePrint(string message, int level = 1)
{
    var formattedMessage = "STDOUTHOOK: " + message;
    Debug.WriteLine(formattedMessage);

    if (options.Verbose >= level)
    {
        Console.Error.WriteLine(formattedMessage);
    }
}

void VerbosePrintError(string message, int level = 1)
{
    var formattedMessage = "STDOUTHOOK: " + message;
    Debug.WriteLine(formattedMessage);

    if (options.Verbose >= level)
    {
        Console.Error.WriteLine(formattedMessage);
    }
}
