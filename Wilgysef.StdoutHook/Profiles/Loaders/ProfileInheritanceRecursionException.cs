using System;

namespace Wilgysef.StdoutHook.Profiles.Loaders
{
    public class ProfileInheritanceRecursionException : Exception
    {
        public string ProfileName { get; }

        public ProfileInheritanceRecursionException(string profileName)
            : base($"Profile contains recursive inherited profile: {profileName}")
        {
            ProfileName = profileName;
        }
    }
}
