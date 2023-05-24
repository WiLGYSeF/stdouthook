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

    [Fact]
    public void Newline()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            MaxFields = 2,
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2"), "123"),
            },
        };
        ShouldRuleBe(rule, "test asdf\r\n", "test 123\r\n");
    }

    [Fact]
    public void Field_Current()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2-4"), "=%Fc="),
            },
        };
        ShouldRuleBe(rule, "test asdf abc  def   ghi", "test =asdf= =abc=  =def=   ghi");
    }

    [Fact]
    public void Field_Current_Multiple()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2"), "=%Fc%Fc="),
            },
        };
        ShouldRuleBe(rule, "test asdf abc", "test =asdfasdf= abc");
    }

    [Fact]
    public void ReplaceAll()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"), "%F3 %F4 %F2");
        ShouldRuleBe(rule, "test asdf abc  def   ghi", "abc def asdf");
    }

    [Fact]
    public void ReplaceAll_FieldRange()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"), "%F(2-4)");
        ShouldRuleBe(rule, "test asdf abc  def   ghi", "asdf abc  def");
    }

    [Fact]
    public void ReplaceAll_Field_OutOfRange()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"), "%F9");
        ShouldRuleBe(rule, "test asdf abc  def   ghi", "");
    }

    [Fact]
    public void ReplaceAll_Field_Invalid()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"), "%Fg");
        ShouldRuleBe(rule, "test asdf abc  def   ghi", "%Fg");

        rule = new FieldSeparatorRule(new Regex(@"\s+"), "%F");
        ShouldRuleBe(rule, "test asdf abc  def   ghi", "%F");
    }

    [Fact]
    public void ReplaceAll_FieldSeparator()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"), "a%F(s3)b");
        ShouldRuleBe(rule, "test asdf abc  def   ghi", "a  b");

        rule = new FieldSeparatorRule(new Regex(@"\s+"), "a%F(S3)b");
        ShouldRuleBe(rule, "test asdf abc  def   ghi", "a  b");
    }

    [Fact]
    public void ReplaceAll_FieldSeparator_OutOfRange()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"), "a%F(s9)b");
        ShouldRuleBe(rule, "test asdf abc  def   ghi", "ab");
    }

    [Fact]
    public void ReplaceAll_FieldSeparator_Range()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"), "a%F(s1-3)b");
        ShouldRuleBe(rule, "test asdf abc  def   ghi", "a%F(s1-3)b");
    }

    [Fact]
    public void ReplaceAll_Current()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"), "%Fc %Fc");
        ShouldRuleBe(rule, "test asdf abc  def   ghi", "test asdf");
    }

    [Fact]
    public void ReplaceAll_Current_MoreThanFieldCount()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"), "%Fc %Fc %Fc");
        ShouldRuleBe(rule, "test asdf", "test asdf ");
    }

    [Fact]
    public void ReplaceAll_Count()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"), "%F(#)");
        ShouldRuleBe(rule, "test asdf abc  def   ghi", "5");
    }

    [Fact]
    public void WithColors()
    {
        var rule = new FieldSeparatorRule(new Regex("---"), "%F1 %F2 %F3");
        ShouldRuleBe(rule, "\x1b[31mtest---asdf---abc", "\x1b[31mtest asdf abc");
    }

    [Fact]
    public void WithColors_Separator()
    {
        var rule = new FieldSeparatorRule(new Regex("---"), "%F1%Fs1%F2 %F3");
        ShouldRuleBe(rule, "test-\x1b[1m--asdf-\x1b[31m-\x1b[0m-abc", "test-\x1b[1m--asdf abc");
    }

    [Fact]
    public void WithColors_ColorEndField()
    {
        var rule = new FieldSeparatorRule(new Regex("---"), "%F1 %F2 %F3");
        ShouldRuleBe(rule, "test\x1b[1m---asdf---abc", "test\x1b[1m asdf abc");
    }

    [Fact]
    public void WithColors_ColorEndSeparator()
    {
        var rule = new FieldSeparatorRule(new Regex("---"), "%F1 %F2 %F3");

        ShouldRuleBe(rule, "test---\x1b[31masdf---abc", "test \x1b[31masdf abc");
        ShouldRuleBe(rule, "test-\x1b[32m--\x1b[31masdf---abc", "test \x1b[31masdf abc");
    }

    [Fact]
    public void TrimNewline()
    {
        var rule = new FieldSeparatorRule(new Regex(" "), "%F1 %F2 %F3")
        {
            TrimNewline = true
        };
        ShouldRuleBe(rule, "a b c\n", "a b c");
    }

    [Fact]
    public void NoContext()
    {
        var rule = new UnconditionalReplaceRule("%Fc");
        ShouldRuleBe(rule, "test", "");

        rule = new UnconditionalReplaceRule("%F1");
        ShouldRuleBe(rule, "test", "");

        rule = new UnconditionalReplaceRule("%Fs1");
        ShouldRuleBe(rule, "test", "");

        rule = new UnconditionalReplaceRule("%F(1-4)");
        ShouldRuleBe(rule, "test", "");

        rule = new UnconditionalReplaceRule("%F(#)");
        ShouldRuleBe(rule, "test", "");
    }

    [Fact]
    public void Copy_ReplaceFields()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            MinFields = 1,
            MaxFields = 2,
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>
            {
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("1"), "a"),
                new KeyValuePair<FieldRangeList, string>(FieldRangeList.Parse("2-*"), "b"),
            },
        };

        using var profile = CreateDummyProfile();
        rule.Build(profile, GetFormatter());

        var copy = (FieldSeparatorRule)rule.Copy();

        copy.MinFields.ShouldBe(rule.MinFields);
        copy.MaxFields.ShouldBe(rule.MaxFields);

        copy.ReplaceFields!.Count.ShouldBe(rule.ReplaceFields.Count);
    }

    [Fact]
    public void Copy_ReplaceAllFormat()
    {
        var rule = new FieldSeparatorRule(new Regex(@"\s+"))
        {
            MinFields = 1,
            MaxFields = 2,
            ReplaceAllFormat = "a",
        };

        using var profile = CreateDummyProfile();
        rule.Build(profile, GetFormatter());

        var copy = (FieldSeparatorRule)rule.Copy();

        copy.MinFields.ShouldBe(rule.MinFields);
        copy.MaxFields.ShouldBe(rule.MaxFields);

        copy.ReplaceAllFormat.ShouldBe(rule.ReplaceAllFormat);
    }
}
