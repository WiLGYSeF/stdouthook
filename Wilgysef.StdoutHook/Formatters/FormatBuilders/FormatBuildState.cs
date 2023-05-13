﻿using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class FormatBuildState
    {
        public string Contents { get; }

        public Profile Profile { get; }

        public FormatBuildState(string contents, Profile profile)
        {
            Contents = contents;
            Profile = profile;
        }
    }
}
