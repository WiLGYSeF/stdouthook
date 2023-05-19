using Wilgysef.StdoutHook.Extensions;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class DataState
    {
        public string Data
        {
            get => _data;
            internal set
            {
                _data = value!;
                DataTrimEndNewline = _data.TrimEndNewline(out _newline);
            }
        }

        public string DataTrimEndNewline { get; internal set; } = null!;

        public string Newline => _newline;

        public bool Stdout { get; }

        public Profile Profile { get; }

        public RuleContext Context { get; private set; } = new RuleContext();

        private string _data = null!;
        private string _newline = null!;

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
