using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests;

public abstract class RuleTestBase
{
    protected static void ShouldRuleBe(Rule rule, string input, string expected)
    {
        ShouldRuleBe(rule, new Formatter(FormatFunctionBuilder.Create()), input, expected);
    }

    private protected static void ShouldRuleBe(Rule rule, Formatter formatter, string input, string expected)
    {
        using var profile = new Profile();
        ShouldRuleBe(profile, rule, formatter, input, expected);
    }

    private protected static void ShouldRuleBe(Profile profile, Rule rule, Formatter formatter, string input, string expected)
    {
        rule.Build(profile, formatter);
        rule.Apply(new DataState(input, true, profile)).ShouldBe(expected);
    }

    private protected static Formatter GetFormatter()
    {
        return new Formatter(FormatFunctionBuilder.Create());
    }

    private protected static Formatter GetFormatter(params FormatBuilder[] formatBuilders)
    {
        return new Formatter(new FormatFunctionBuilder(formatBuilders));
    }

    protected static Profile CreateDummyProfile()
    {
        return new Profile();
    }

    private protected static DataState CreateDummyDataState()
    {
        return new DataState(new Profile());
    }

    private protected static DataState CreateDummyDataState(string data, bool stdout)
    {
        return new DataState(data, stdout, new Profile());
    }
}
