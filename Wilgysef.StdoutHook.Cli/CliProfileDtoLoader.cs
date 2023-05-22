using Wilgysef.StdoutHook.Loggers;
using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;

namespace Wilgysef.StdoutHook.Cli;

internal class CliProfileDtoLoader
{
    public async Task<List<ProfileDto>> LoadProfileDtosAsync(string profileDir)
    {
        var jsonLoader = new JsonProfileDtoLoader();

        var loaders = new Dictionary<string, IReadOnlyList<ProfileDtoLoader>>
        {
            [""] = new[] { jsonLoader },
            [".json"] = new[] { jsonLoader },
            [".txt"] = new[] { jsonLoader },
            // [".yaml"] = new[] { yamlLoader },
            // [".yml"] = new[] { yamlLoader },
        };

        return await LoadProfileDtosFromDirectoryAsync(loaders, profileDir);
    }

    private static async Task<List<ProfileDto>> LoadProfileDtosFromDirectoryAsync(
        IReadOnlyDictionary<string, IReadOnlyList<ProfileDtoLoader>> loadersByExtension,
        string path)
    {
        var profiles = new List<ProfileDto>();
        var files = Directory.GetFiles(path);

        for (var i = 0; i < files.Length; i++)
        {
            var file = files[i];
            var extension = Path.GetExtension(file);

            if (file[0] == '.' || !loadersByExtension.TryGetValue(extension, out var loaders))
            {
                continue;
            }

            try
            {
                using var stream = File.Open(file, FileMode.Open, FileAccess.Read);
                IList<ProfileDto>? dtos = null;
                var exceptions = new List<Exception>();

                foreach (var loader in loaders)
                {
                    try
                    {
                        dtos = await loader.LoadProfileDtosAsync(stream);
                        break;
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                        stream.Position = 0;
                    }
                }

                if (dtos == null)
                {
                    throw exceptions[0];
                }

                profiles.AddRange(dtos);

                Shared.VerbosePrint($"loaded profiles from: {file}");
            }
            catch (Exception ex)
            {
                GlobalLogger.Error($"failed to load profiles: {ex.Message}: {file}");
            }
        }

        return profiles;
    }
}
