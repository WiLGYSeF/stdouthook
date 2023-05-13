using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class FilterRule : Rule
    {
        public override sealed bool Filter { get => true; protected set => base.Filter = value; }

        internal override string Apply(DataState state)
        {
            return state.Data!;
        }
    }
}
