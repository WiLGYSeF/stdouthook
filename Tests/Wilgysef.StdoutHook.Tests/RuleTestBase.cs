using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests;

public abstract class RuleTestBase
{
    protected static void ShouldRuleBe(Rule rule, string input, string expected)
    {
        ShouldRuleBe(rule, GetFormatter(FormatFunctionBuilder.FormatBuilders), input, expected);
    }

    protected private static void ShouldRuleBe(Rule rule, Formatter formatter, string input, string expected)
    {
        var profile = new Profile(new ProfileState());
        rule.Build(profile, formatter);
        rule.Apply(new DataState(input, true, profile)).ShouldBe(expected);
    }

    private protected static Formatter GetFormatter(params FormatBuilder[] formatBuilders)
    {
        return new Formatter(new FormatFunctionBuilder(formatBuilders));
    }

    private protected static DataState CreateDummyDataState()
    {
        return new DataState(new Profile(new ProfileState()));
    }

    private protected static DataState CreateDummyDataState(string data, bool stdout)
    {
        return new DataState(data, stdout, new Profile(new ProfileState()));
    }
}
