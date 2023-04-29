using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Formatters.FormatBuilders;

namespace Wilgysef.StdoutHook.Tests;

public abstract class RuleTestBase
{
    private protected static Formatter GetFormatter(params FormatBuilder[] formatBuilders)
    {
        return new Formatter(new FormatFunctionBuilder(formatBuilders));
    }
}
