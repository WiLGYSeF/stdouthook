namespace Wilgysef.StdoutHook.Profiles
{
    internal class DataState
    {
        // TODO: not null?
        public string? Data { get; internal set; }

        public bool Stdout { get; }

        public ProfileState ProfileState { get; }

        public DataState(string data, bool stdout, ProfileState profileState)
        {
            Data = data;
            Stdout = stdout;
            ProfileState = profileState;
        }

        public DataState(ProfileState profileState)
        {
            ProfileState = profileState;
        }
    }
}
