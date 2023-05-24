using System.Text;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class RegexGroupRuleTest : RuleTestBase
{
    private static readonly int MaximumGroupCount = 32;

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
    public void Color()
    {
        var rule = new RegexGroupRule(new Regex(@"a([a-z]+)f"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1"), "0%G1"),
        });
        ShouldRuleBe(rule, "test\x1b[33m as\x1b[31mdf \x1b[34mabc", "test\x1b[33m a0s\x1b[31mdf \x1b[34mabc");
    }

    [Fact]
    public void Color_Multiple()
    {
        var rule = new RegexGroupRule(new Regex(@"\d{2}:(\d{2}):\d{2}"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1"), "%Cblue%Gc%Cz"),
        });
        ShouldRuleBe(rule, "2000-01-02 03:04:05 06:07:08.999 \x1b[31mtest\x1b[0m", "2000-01-02 03:\x1b[34m04\x1b[0m:05 06:\x1b[34m07\x1b[0m:08.999 \u001b[31mtest\u001b[0m");
    }

    [Fact]
    public void Current()
    {
        var rule = new RegexGroupRule(new Regex(@"([0-9]+)\.([0-9]+)"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1,2"), "=%Gc="),
        });
        ShouldRuleBe(rule, "123.456", "=123=.=456=");
    }

    [Fact]
    public void Current_Multiple()
    {
        var rule = new RegexGroupRule(new Regex(@"([0-9]+)\.([0-9]+)"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1,2"), "=%Gc%Gc="),
        });
        ShouldRuleBe(rule, "123.456", "=123123=.=456456=");
    }

    [Fact]
    public void Named()
    {
        var rule = new RegexGroupRule(new Regex(@"(?<test>[0-9]+)"), new Dictionary<string, string>
        {
            ["test"] = "abc",
        });
        ShouldRuleBe(rule, "abc123", "abcabc");

        rule = new RegexGroupRule(new Regex(@"(?<test>[0-9]+)"), null!, new Dictionary<string, string>
        {
            ["test"] = "abc",
            ["notexist"] = "a",
        });
        ShouldRuleBe(rule, "abc123", "abcabc");
    }

    [Fact]
    public void Nested()
    {
        var rule = new RegexGroupRule(new Regex(@"a(b(c))d"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1"), "z"),
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2"), "x"),
        });
        ShouldRuleBe(rule, "abcd", "azd");
    }

    [Fact]
    public void OutOfRange()
    {
        var groups = 40;
        var groupNumber = 35;
        groupNumber.ShouldBeGreaterThan(MaximumGroupCount);

        var regexBuilder = new StringBuilder();
        var builder = new StringBuilder();
        var expectedBuilder = new StringBuilder();

        for (var i = 0; i < groups; i++)
        {
            regexBuilder.Append("(.)");
            builder.Append('a');

            expectedBuilder.Append(i + 1 == groupNumber ? 'b' : 'a');
        }

        var rule = new RegexGroupRule(new Regex(regexBuilder.ToString()), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse(groupNumber.ToString()), "b"),
        });
        ShouldRuleBe(rule, builder.ToString(), expectedBuilder.ToString());
    }

    [Fact]
    public void Newline()
    {
        var rule = new RegexGroupRule(new Regex(@"(?<test>[0-9]+)$"), new Dictionary<string, string>
        {
            ["test"] = "abc",
        });
        ShouldRuleBe(rule, "abc123\n", "abcabc\n");
    }

    [Fact]
    public void Group_NotExist()
    {
        var rule = new RegexGroupRule(new Regex(@"a([a-z]+)f"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("4"), "123"),
        });
        ShouldRuleBe(rule, "test asdf abc", "test asdf abc");
    }

    [Fact]
    public void NoGroup()
    {
        var rule = new RegexGroupRule(new Regex(@"([0-9]+)"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1"), "%G"),
        });
        ShouldRuleBe(rule, "abc123", "abc%G");
    }

    [Fact]
    public void TrimNewline()
    {
        var rule = new RegexGroupRule(new Regex(@"([0-9]+)"), new List<KeyValuePair<FieldRangeList, string>>())
        {
            TrimNewline = true,
        };
        ShouldRuleBe(rule, "abc123\n", "abc123");
    }

    [Fact]
    public void ReplaceZero()
    {
        var rule = new RegexGroupRule(new Regex(@"[0-9]+"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1"), "b"),
        });
        ShouldRuleBe(rule, "abc123", "abcb");
    }

    [Fact]
    public void NoContext()
    {
        var rule = new UnconditionalReplaceRule("%Gc %G0");
        ShouldRuleBe(rule, "abc123", " ");
    }

    [Fact]
    public void Copy()
    {
        var rule = new RegexGroupRule(new Regex("a"))
        {
            ReplaceGroups = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1"), "asdf"),
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2-*"), "qweq"),
            },
            ReplaceNamedGroups = new Dictionary<string, string>
            {
                ["a"] = "b",
            },
        };

        using var profile = CreateDummyProfile();
        rule.Build(profile, GetFormatter());

        var copy = (RegexGroupRule)rule.Copy();
        copy.Regex.ToString().ShouldBe(rule.Regex.ToString());
        copy.ReplaceGroups!.Count.ShouldBe(2);
        copy.ReplaceNamedGroups!.Count.ShouldBe(1);
    }
}
