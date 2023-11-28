using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules;

public class FilterRule : Rule
{
    /// <inheritdoc/>
    public override sealed bool Filter
    {
        get => true;
        protected set { }
    }

    /// <inheritdoc/>
    internal override string Apply(DataState state)
    {
        // should not be reached
        return state.Data;
    }

    /// <inheritdoc/>
    protected override Rule CopyInternal()
    {
        return new FilterRule();
    }
}
