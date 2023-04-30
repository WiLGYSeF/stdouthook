using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class FormatBuildState
    {
        public string Contents { get; }

        public ProfileState ProfileState { get; }

        public FormatBuildState(string contents, ProfileState profileState)
        {
            Contents = contents;
            ProfileState = profileState;
        }
    }
}
