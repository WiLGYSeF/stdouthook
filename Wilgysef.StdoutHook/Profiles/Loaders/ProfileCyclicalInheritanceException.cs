﻿namespace Wilgysef.StdoutHook.Profiles.Loaders
{
    public class ProfileCyclicalInheritanceException : ProfileLoaderException
    {
        public string ProfileName { get; }

        public ProfileCyclicalInheritanceException(string profileName)
            : base($"Profile contains cyclical inherited profile: {profileName}")
        {
            ProfileName = profileName;
        }
    }
}
