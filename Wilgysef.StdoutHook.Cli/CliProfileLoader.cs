using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;

namespace Wilgysef.StdoutHook.Cli;

internal class CliProfileLoader
{
    public Profile? LoadProfile(
        IReadOnlyList<ProfileDto> profileDtos,
        string? profileName,
        string command,
        IReadOnlyList<string> arguments)
    {
        var commandPaths = new CommandLocator().LocateCommand(command);
        var fullCommandPath = commandPaths.FirstOrDefault();

        var loader = new ProfileLoader();
        var picker = new ProfileDtoPicker();
        return loader.LoadProfile(
            profileDtos,
            profiles => picker.PickProfileDto(
                profiles,
                profileName: profileName,
                command: command,
                fullCommandPath: fullCommandPath,
                arguments: arguments),
            throwIfInheritedProfileNotFound: false);
    }
}
