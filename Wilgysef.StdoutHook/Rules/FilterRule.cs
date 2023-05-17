using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class FilterRule : Rule
    {
        public override sealed bool Filter { get => true; protected set { } }

        internal override string Apply(DataState state)
        {
            // should not be reached
            return state.Data!;
        }
    }
}
