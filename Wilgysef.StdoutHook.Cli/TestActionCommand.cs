using Wilgysef.StdoutHook.ActionCommands;

namespace Wilgysef.StdoutHook.Cli;

internal class TestActionCommand : ActionCommand
{
    private readonly Func<string, string> _func;

    public TestActionCommand(Func<string, string> func)
    {
        _func = func;
    }

    public override string Execute(ActionCommandState state)
    {
        return _func(state.Data);
    }

    public override bool Predicate(ActionCommandState state)
    {
        return true;
    }
}
