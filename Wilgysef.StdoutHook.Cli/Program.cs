// TODO: use https://github.com/microsoft/vs-pty.net

using CommandLine;
using System.Diagnostics;
using Wilgysef.StdoutHook.Cli;
using Wilgysef.StdoutHook.CommandLocator;
using Wilgysef.StdoutHook.Loggers;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;

const string LogName = ".stdouthook.log";

try
{
    var argParseResult = new Parser(parser =>
    {
        parser.EnableDashDash = true;
        parser.HelpWriter = Console.Error;
    })
    // TODO: replace test code
        .ParseArguments<Options>(new[] { "-v", "--profile", "test", "python", "D:\\projects\\stdouthook\\Wilgysef.StdoutHook\\testproc.py" });

    if (argParseResult.Tag == ParserResultType.NotParsed)
    {
        throw new ProgramSetupException();
    }

    Shared.Options = argParseResult.Value;

    var configDir = Shared.Options.ConfigDir;
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
            return 1;
        }
    }

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

    using var logStream = new StreamWriter(GetLogStream(
        configDir != null
            ? Path.Combine(configDir, LogName)
            : null));
    GlobalLogger.Logger = new Logger(logStream);

    var profileName = Shared.Options.ProfileName;
    var command = Shared.Options.Arguments[0];
    var commandPaths = new CommandLocator().LocateCommand(command);
    var fullCommandPath = commandPaths.FirstOrDefault();
    var arguments = Shared.Options.Arguments.Skip(1).ToArray();

    var cliProfileLoader = new CliProfileDtoLoader();
    var profileDtos = configDir != null
        ? await cliProfileLoader.LoadProfileDtosAsync(configDir)
        : new List<ProfileDto>();

    var loader = new ProfileLoader();
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

    if (profileLoaded)
    {
        Shared.VerbosePrint($"using profile: {profile!.ProfileName ?? "<unnamed>"}");
    }
    else
    {
        // TODO: quote
        Shared.VerbosePrint($"no profile selected");
    }

    try
    {
        profile?.Build();
    }
    catch (Exception ex)
    {
        GlobalLogger.Error(ex, "failed to build profile");
        profileLoaded = false;
    }

    TextWriter? outputStreamWriter = null;
    TextWriter? errorStreamWriter = null;

    if (Shared.Options.Stdout != null)
    {
        try
        {
            outputStreamWriter = new StreamWriter(new FileStream(
                Shared.Options.Stdout,
                Shared.Options.StdoutAppend ? FileMode.Append : FileMode.Open,
                FileAccess.Write));
        }
        catch (Exception ex)
        {
            Shared.ErrorEx(
                ex,
                $"could not open stdout file: {Shared.Options.Stdout}",
                $"could not open stdout file: {ex.Message}: {Shared.Options.Stdout}");
            return 1;
        }
    }

    if (Shared.Options.Stderr != null)
    {
        try
        {
            errorStreamWriter = new StreamWriter(new FileStream(
                Shared.Options.Stderr,
                Shared.Options.StderrAppend ? FileMode.Append : FileMode.Open,
                FileAccess.Write));
        }
        catch (Exception ex)
        {
            Shared.ErrorEx(
                ex,
                $"could not open stderr file: {Shared.Options.Stderr}",
                $"could not open stderr file: {ex.Message}: {Shared.Options.Stderr}");
            return 1;
        }
    }

    Process? process;

    try
    {
        process = Process.Start(CreateProcessStartInfo(profile, command, arguments));
        if (process == null)
        {
            Shared.Error($"process could not be started: {command}");
            return 1;
        }
    }
    catch (Exception ex)
    {
        Shared.ErrorEx(
            ex,
            $"process could not be started: {command}",
            $"process could not be started: {ex.Message}: {command}");
        return 1;
    }

    // TODO: cleanup
    using var __process = process;

    Task? readStreamTask = null;
    var cancellationTokenSource = new CancellationTokenSource();

    if (profileLoaded)
    {
        profile!.State.SetProcess(process);

        var flush = profile.Flush || Shared.Options.Flush;
        var bufferSize = flush ? 1 : Shared.Options.BufferSize;
        outputStreamWriter ??= new CustomStreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding, bufferSize);
        errorStreamWriter ??= new CustomStreamWriter(Console.OpenStandardError(), Console.OutputEncoding, bufferSize);

        var streamOutputHandler = new StreamOutputHandler(
            profile,
            process.StandardOutput,
            process.StandardError,
            outputStreamWriter,
            errorStreamWriter);

        readStreamTask = streamOutputHandler.ReadLinesAsync(cancellationToken: cancellationTokenSource.Token);

        if (!flush)
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
    }

    // TODO: remove test code
    Console.ReadLine();
    process.Kill();

    process.WaitForExit();
    cancellationTokenSource.Cancel();

    outputStreamWriter?.Dispose();
    errorStreamWriter?.Dispose();

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

    return process.HasExited
        ? process.ExitCode
        : 0;
}
catch (ProgramSetupException setupException)
{
    return setupException.ExitCode;
}
catch (Exception ex)
{
    return 1;
}

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
