using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class RegexGroupRuleTest : RuleTestBase
{
    private static readonly int MaximumGroupCount = 16;

    [Fact]
    public void Replace()
    {
        var rule = new RegexGroupRule(new Regex(@"a([a-z]+)f"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1"), "123"),
        });
        ShouldRuleBe(rule, "test asdf abc", "test a123f abc");

        rule = new RegexGroupRule(new Regex(@"a([a-z]+)f"));
        rule.ReplaceGroups!.Add(new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1"), "123"));
        ShouldRuleBe(rule, "test asdf abc", "test a123f abc");
    }

    [Fact]
    public void NoMatch()
    {
        var rule = new RegexGroupRule(new Regex(@"zxcv"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1"), "123"),
        });
        ShouldRuleBe(rule, "test asdf abc", "test asdf abc");
    }

    [Fact]
    public void Replace_Color()
    {
        var rule = new RegexGroupRule(new Regex(@"a([a-z]+)f"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1"), "0%G1"),
        });
        ShouldRuleBe(rule, "test as\x1b[31mdf abc", "test a0s\x1b[31mdf abc");
    }

    [Fact]
    public void Replace_Current()
    {
        var rule = new RegexGroupRule(new Regex(@"([0-9]+)\.([0-9]+)"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1,2"), "=%Gc="),
        });
        ShouldRuleBe(rule, "123.456", "=123=.=456=");
    }

    [Fact]
    public void Replace_Current_Multiple()
    {
        var rule = new RegexGroupRule(new Regex(@"([0-9]+)\.([0-9]+)"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1,2"), "=%Gc%Gc="),
        });
        ShouldRuleBe(rule, "123.456", "=123123=.=456456=");
    }

    [Fact]
    public void ReplaceAll()
    {
        var rule = new RegexGroupRule(new Regex(@"a([a-z]+)f"), "aaa %G1");

        ShouldRuleBe(rule, "test asdf abc", "aaa sd");
    }

    [Fact]
    public void ReplaceAll_NoMatch()
    {
        var rule = new RegexGroupRule(new Regex(@"zxcv"), "aaa");

        ShouldRuleBe(rule, "test asdf abc", "test asdf abc");
    }

    [Fact]
    public void ReplaceAll_OutOfRange()
    {
        var rule = new RegexGroupRule(new Regex(@"([0-9]+)\.([0-9]+)"), "%G4");
        ShouldRuleBe(rule, "123.456", "");
    }

    [Fact]
    public void ReplaceAll_Replace_Color()
    {
        var rule = new RegexGroupRule(new Regex(@"a([a-z]+)f"), "%G1");

        ShouldRuleBe(rule, "test as\x1b[31mdf abc", "s\x1b[31md");
    }

    [Fact]
    public void ReplaceAll_Current()
    {
        var rule = new RegexGroupRule(new Regex(@"([0-9]+)\.([0-9]+)"), "=%Gc=.=%Gc=");
        ShouldRuleBe(rule, "123.456", "=123=.=456=");
    }

    private static void ShouldRuleBe(Rule rule, string input, string expected)
    {
        var state = new ProfileState();
        rule.Build(state, GetFormatter(FormatFunctionBuilder.FormatBuilders));
        rule.Apply(new DataState(input, true, state)).ShouldBe(expected);
    }
}
