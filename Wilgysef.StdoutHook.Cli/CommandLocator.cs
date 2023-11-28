using System.Text;

namespace Wilgysef.StdoutHook.Cli;

public class CommandLocator
{
    private readonly Func<string, bool> _fileExists;

    public CommandLocator(Func<string, bool>? fileExists = null)
    {
        _fileExists = fileExists
            ?? File.Exists;
    }

    public List<string> LocateCommand(string command)
    {
        var envPathsArr = Environment.GetEnvironmentVariable("PATH")
            ?.Split(Path.PathSeparator)
            ?? Array.Empty<string>();
        var envPathExts = Environment.GetEnvironmentVariable("PATHEXT")
            ?.Split(Path.PathSeparator)
            ?? Array.Empty<string>();

        var envPaths = new List<string>(envPathsArr.Length + 1)
        {
            Environment.CurrentDirectory,
        };
        envPaths.AddRange(envPathsArr);

        return LocateCommand(command, envPaths, envPathExts);
    }

    public List<string> LocateCommand(
        string command,
        IReadOnlyList<string> pathsToCheck,
        IReadOnlyList<string>? extensions)
    {
        var paths = new List<string>();
        var builder = new StringBuilder();

        for (var i = 0; i < pathsToCheck.Count; i++)
        {
            var fullPath = Path.Combine(pathsToCheck[i], command);

            if (_fileExists(fullPath) && !paths.Contains(fullPath))
            {
                paths.Add(fullPath);
            }

            if (extensions != null && extensions.Count > 0)
            {
                var prefix = builder
                    .Clear()
                    .Append(fullPath)
                    .Length;

                for (var j = 0; j < extensions.Count; j++)
                {
                    var fullPathWithExt = builder
                        .Remove(prefix, builder.Length - prefix)
                        .Append(extensions[j])
                        .ToString();

                    if (_fileExists(fullPathWithExt) && !paths.Contains(fullPathWithExt))
                    {
                        paths.Add(fullPathWithExt);
                    }
                }
            }
        }

        return paths;
    }
}
