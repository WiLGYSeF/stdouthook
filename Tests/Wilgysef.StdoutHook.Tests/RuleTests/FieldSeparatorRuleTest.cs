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

        rule.Build(new ProfileState(), GetFormatter());
        rule.Apply(CreateDataState("test asdf")).ShouldBe("123 asdf");
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

        rule.Build(new ProfileState(), GetFormatter());
        rule.Apply(CreateDataState("test asdf abc")).ShouldBe("test 123 abc");
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

        rule.Build(new ProfileState(), GetFormatter());
        rule.Apply(CreateDataState("test asdf abc")).ShouldBe("test asdf 123");
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

        rule.Build(new ProfileState(), GetFormatter());
        rule.Apply(CreateDataState("test asdf abc  def   ghi")).ShouldBe("test 123 abc  456   ghi");
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

        rule.Build(new ProfileState(), GetFormatter());
        rule.Apply(CreateDataState("test asdf abc  def   ghi")).ShouldBe("test 123 123  123   ghi");
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

        rule.Build(new ProfileState(), GetFormatter());
        rule.Apply(CreateDataState("test asdf abc")).ShouldBe("test 123 abc");
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

        rule.Build(new ProfileState(), GetFormatter());
        rule.Apply(CreateDataState(data)).ShouldBe(expected);
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

        rule.Build(new ProfileState(), GetFormatter());
        rule.Apply(CreateDataState("test asdf")).ShouldBe("test asdf");

        rule = new FieldSeparatorRule
        {
            SeparatorRegex = new Regex(@"\s+"),
            MaxFields = 1,
            ReplaceFields = new List<KeyValuePair<FieldRange, string>>
            {
                new KeyValuePair<FieldRange, string>(new FieldRange(2), "123"),
            },
        };

        rule.Build(new ProfileState(), GetFormatter());
        rule.Apply(CreateDataState("test asdf")).ShouldBe("test asdf");
    }

    private static DataState CreateDataState(string data)
    {
        return new DataState(data, true, new ProfileState());
    }
}
