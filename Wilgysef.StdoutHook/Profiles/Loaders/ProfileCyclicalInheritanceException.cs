namespace Wilgysef.StdoutHook.Profiles.Loaders;

public class ProfileCyclicalInheritanceException : ProfileLoaderException
{
    public ProfileCyclicalInheritanceException(string profileName)
        : base($"Profile contains cyclical inherited profile: {profileName}")
    {
        ProfileName = profileName;
    }

    public string ProfileName { get; }
}
