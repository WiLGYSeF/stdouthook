using System;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class DataState
    {
        // TODO: not null?
        public string? Data { get; internal set; }

        public bool Stdout { get; }

        public Profile Profile { get; }

        public RuleContext Context { get; private set; } = new RuleContext();

        public DataState(string data, bool stdout, Profile profile)
        {
            Data = data;
            Stdout = stdout;
            Profile = profile;
        }

        public DataState(Profile profile)
        {
            Profile = profile;
        }

        public void ResetContext()
        {
            Context = new RuleContext();
        }
    }
}
