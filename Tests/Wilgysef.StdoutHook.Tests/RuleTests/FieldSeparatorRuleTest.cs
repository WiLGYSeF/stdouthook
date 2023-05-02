using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class FieldSeparatorRuleTest : RuleTestBase
{
    private static readonly int MaximumFieldCount = 128;

    [Fact]
    public void FirstField()
    {
        var replaceFields = new List<KeyValuePair<FieldRangeList, string>>
        {
            new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1"), "123"),
        };

        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            ReplaceFields = replaceFields,
        };

        ShouldRuleBe(rule, "test asdf", "123 asdf");

        rule = new FieldSeparatorRule(new Regex(@"\s+"), replaceFields);
        ShouldRuleBe(rule, "test asdf", "123 asdf");
    }

    [Fact]
    public void MiddleField()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2"), "123"),
            },
        };

        ShouldRuleBe(rule, "test asdf abc", "test 123 abc");
    }

    [Fact]
    public void LastField()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("3"), "123"),
            },
        };

        ShouldRuleBe(rule, "test asdf abc", "test asdf 123");
    }

    [Fact]
    public void MultipleFields()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2"), "123"),
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("4"), "456"),
            },
        };

        ShouldRuleBe(rule, "test asdf abc  def   ghi", "test 123 abc  456   ghi");
    }

    [Fact]
    public void FieldRange()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2-4"), "123"),
            },
        };

        ShouldRuleBe(rule, "test asdf abc  def   ghi", "test 123 123  123   ghi");
    }

    [Fact]
    public void FieldRange_Infinite()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2-*"), "123"),
            },
        };

        ShouldRuleBe(rule, "test asdf abc  def   ghi", "test 123 123  123   123");
    }

    [Fact]
    public void Override()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2"), "123"),
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2"), "456"),
            },
        };

        ShouldRuleBe(rule, "test asdf abc", "test 123 abc");
    }

    [Fact]
    public void ExceedMaxFieldCount()
    {
        var fields = 200;
        var replaceField = 180;

        fields.ShouldBeGreaterThan(MaximumFieldCount);

        var data = string.Join(" ", Enumerable.Range(0, fields).Select(_ => "test"));
        var expected = string.Join(" ", Enumerable.Range(0, fields).Select((_, i) => i == replaceField ? "123" : "test"));

        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse($"{replaceField}"), "123"),
            },
        };

        ShouldRuleBe(rule, data, expected);
    }

    [Fact]
    public void ExceedMaxFieldCount_InfiniteMax()
    {
        var fields = 200;
        var replaceField = 180;

        fields.ShouldBeGreaterThan(MaximumFieldCount);

        var data = string.Join(" ", Enumerable.Range(0, fields).Select(_ => "test"));
        var expected = string.Join(" ", Enumerable.Range(0, fields).Select((_, i) => i >= replaceField ? "123" : "test"));

        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse($"{replaceField}-*"), "123"),
            },
        };

        ShouldRuleBe(rule, data, expected);
    }

    [Fact]
    public void OutOfRange()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            MinFields = 3,
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2"), "123"),
            },
        };

        ShouldRuleBe(rule, "test asdf", "test asdf");

        rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            MaxFields = 1,
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2"), "123"),
            },
        };

        ShouldRuleBe(rule, "test asdf", "test asdf");
    }

    private static void ShouldRuleBe(Rule rule, string input, string expected)
    {
        var state = new ProfileState();
        rule.Build(state, GetFormatter());
        rule.Apply(new DataState(input, true, state)).ShouldBe(expected);
    }
}
