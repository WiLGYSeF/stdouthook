using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class FieldSeparatorRuleTest : RuleTestBase
{
    [Fact]
    public void FirstField()
    {
        var rule = new FieldSeparatorRule
        {
            SeparatorRegex = new Regex(@"\s+"),
            ReplaceFields = new List<KeyValuePair<FieldRange, string>>
            {
                new KeyValuePair<FieldRange, string>(new FieldRange(1), "123"),
            },
        };

        rule.Build(GetFormatter());
        rule.Apply("test asdf", true, new ProfileState()).ShouldBe("123 asdf");
    }

    [Fact]
    public void MiddleField()
    {
        var rule = new FieldSeparatorRule
        {
            SeparatorRegex = new Regex(@"\s+"),
            ReplaceFields = new List<KeyValuePair<FieldRange, string>>
            {
                new KeyValuePair<FieldRange, string>(new FieldRange(2), "123"),
            },
        };

        rule.Build(GetFormatter());
        rule.Apply("test asdf abc", true, new ProfileState()).ShouldBe("test 123 abc");
    }

    [Fact]
    public void LastField()
    {
        var rule = new FieldSeparatorRule
        {
            SeparatorRegex = new Regex(@"\s+"),
            ReplaceFields = new List<KeyValuePair<FieldRange, string>>
            {
                new KeyValuePair<FieldRange, string>(new FieldRange(3), "123"),
            },
        };

        rule.Build(GetFormatter());
        rule.Apply("test asdf abc", true, new ProfileState()).ShouldBe("test asdf 123");
    }

    [Fact]
    public void MultipleFields()
    {
        var rule = new FieldSeparatorRule
        {
            SeparatorRegex = new Regex(@"\s+"),
            ReplaceFields = new List<KeyValuePair<FieldRange, string>>
            {
                new KeyValuePair<FieldRange, string>(new FieldRange(2), "123"),
                new KeyValuePair<FieldRange, string>(new FieldRange(4), "456"),
            },
        };

        rule.Build(GetFormatter());
        rule.Apply("test asdf abc  def   ghi", true, new ProfileState()).ShouldBe("test 123 abc  456   ghi");
    }

    [Fact]
    public void FieldRange()
    {
        var rule = new FieldSeparatorRule
        {
            SeparatorRegex = new Regex(@"\s+"),
            ReplaceFields = new List<KeyValuePair<FieldRange, string>>
            {
                new KeyValuePair<FieldRange, string>(new FieldRange(2, 4), "123"),
            },
        };

        rule.Build(GetFormatter());
        rule.Apply("test asdf abc  def   ghi", true, new ProfileState()).ShouldBe("test 123 123  123   ghi");
    }

    [Fact]
    public void Override()
    {
        var rule = new FieldSeparatorRule
        {
            SeparatorRegex = new Regex(@"\s+"),
            ReplaceFields = new List<KeyValuePair<FieldRange, string>>
            {
                new KeyValuePair<FieldRange, string>(new FieldRange(2), "123"),
                new KeyValuePair<FieldRange, string>(new FieldRange(2), "456"),
            },
        };

        rule.Build(GetFormatter());
        rule.Apply("test asdf abc", true, new ProfileState()).ShouldBe("test 123 abc");
    }

    [Fact]
    public void ExceedMaxFieldCount()
    {
        var fields = 200;
        var replaceField = 180;

        var data = string.Join(" ", Enumerable.Range(0, fields).Select(_ => "test"));
        var expected = string.Join(" ", Enumerable.Range(0, fields).Select((_, i) => i == replaceField ? "123" : "test"));

        var rule = new FieldSeparatorRule
        {
            SeparatorRegex = new Regex(@"\s+"),
            ReplaceFields = new List<KeyValuePair<FieldRange, string>>
            {
                new KeyValuePair<FieldRange, string>(new FieldRange(replaceField), "123"),
            },
        };

        rule.Build(GetFormatter());
        rule.Apply(data, true, new ProfileState()).ShouldBe(expected);
    }

    [Fact]
    public void OutOfRange()
    {
        var rule = new FieldSeparatorRule
        {
            SeparatorRegex = new Regex(@"\s+"),
            MinFields = 3,
            ReplaceFields = new List<KeyValuePair<FieldRange, string>>
            {
                new KeyValuePair<FieldRange, string>(new FieldRange(2), "123"),
            },
        };

        rule.Build(GetFormatter());
        rule.Apply("test asdf", true, new ProfileState()).ShouldBe("test asdf");

        rule = new FieldSeparatorRule
        {
            SeparatorRegex = new Regex(@"\s+"),
            MaxFields = 1,
            ReplaceFields = new List<KeyValuePair<FieldRange, string>>
            {
                new KeyValuePair<FieldRange, string>(new FieldRange(2), "123"),
            },
        };

        rule.Build(GetFormatter());
        rule.Apply("test asdf", true, new ProfileState()).ShouldBe("test asdf");
    }
}
