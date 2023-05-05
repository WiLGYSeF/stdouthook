using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class RegexRuleTest : RuleTestBase
{
    [Fact]
    public void Replace()
    {
        var rule = new RegexRule(new Regex(@"a([a-z]+)f"), "aaa %G1");

        ShouldRuleBe(rule, "test asdf abc", "aaa sd");
    }

    [Fact]
    public void NoMatch()
    {
        var rule = new RegexRule(new Regex(@"zxcv"), "aaa");

        ShouldRuleBe(rule, "test asdf abc", "test asdf abc");
    }

    [Fact]
    public void Replace_Color()
    {
        var rule = new RegexRule(new Regex(@"a([a-z]+)f"), "%G1");

        ShouldRuleBe(rule, "test as\x1b[31mdf abc", "s\x1b[31md");
    }

    private static void ShouldRuleBe(Rule rule, string input, string expected)
    {
        var state = new ProfileState();
        rule.Build(state, GetFormatter(FormatFunctionBuilder.FormatBuilders));
        rule.Apply(new DataState(input, true, state)).ShouldBe(expected);
    }
}
