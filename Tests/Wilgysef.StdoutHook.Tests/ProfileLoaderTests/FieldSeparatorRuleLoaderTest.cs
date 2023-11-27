using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.ProfileLoaderTests;

public class FieldSeparatorRuleLoaderTest
{
    [Fact]
    public void FieldSeparator_ReplaceFields_List()
    {
        var loader = new RuleLoader();
        var rule = (FieldSeparatorRule)loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
            ReplaceFields = new List<object?> { "a", "b" },
        });

        rule.SeparatorExpression.ToString().ShouldBe(@"\s+");

        rule.ReplaceFields!.Count.ShouldBe(2);
        rule.ReplaceFields[0].Key.ToString().ShouldBe("1");
        rule.ReplaceFields[0].Value.ShouldBe("a");
        rule.ReplaceFields[1].Key.ToString().ShouldBe("2");
        rule.ReplaceFields[1].Value.ShouldBe("b");
    }

    [Fact]
    public void FieldSeparator_ReplaceFields_Object()
    {
        var loader = new RuleLoader();
        var rule = (FieldSeparatorRule)loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
            ReplaceFields = new Dictionary<string, object?>
            {
                ["1"] = "a",
                ["2"] = "b",
            },
        });

        rule.SeparatorExpression.ToString().ShouldBe(@"\s+");

        rule.ReplaceFields!.Count.ShouldBe(2);
        rule.ReplaceFields[0].Key.ToString().ShouldBe("1");
        rule.ReplaceFields[0].Value.ShouldBe("a");
        rule.ReplaceFields[1].Key.ToString().ShouldBe("2");
        rule.ReplaceFields[1].Value.ShouldBe("b");
    }

    [Fact]
    public void FieldSeparator_ReplaceAllFormat()
    {
        var loader = new RuleLoader();
        var rule = (FieldSeparatorRule)loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
            ReplaceAllFormat = "test",
        });

        rule.SeparatorExpression.ToString().ShouldBe(@"\s+");

        rule.ReplaceAllFormat.ShouldBe("test");
    }

    [Fact]
    public void FieldSeparator_Range()
    {
        var loader = new RuleLoader();
        var rule = (FieldSeparatorRule)loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
            MinFields = 1,
            MaxFields = 4,
            ReplaceAllFormat = "test",
        });

        rule.SeparatorExpression.ToString().ShouldBe(@"\s+");

        rule.MinFields.ShouldBe(1);
        rule.MaxFields.ShouldBe(4);
    }

    [Fact]
    public void FieldSeparator_SeparatorExpression_Split()
    {
        var loader = new RuleLoader();
        var rule = (FieldSeparatorRule)loader.LoadRule(new RuleDto
        {
            SeparatorExpression = new List<object?> { "a", "b" },
            ReplaceAllFormat = "test",
        });

        rule.SeparatorExpression.ToString().ShouldBe("ab");
    }

    [Fact]
    public void FieldSeparator_Invalid()
    {
        var loader = new RuleLoader();

        Should.Throw<InvalidRuleException>(() => loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
        }));

        Should.Throw<InvalidRuleException>(() => loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
            ReplaceFields = new List<object?> { "a", 1 },
        }));

        Should.Throw<InvalidRuleException>(() => loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
            ReplaceFields = new Dictionary<string, object?>
            {
                ["a"] = "1",
                ["b"] = 2,
            },
        }));
    }
}
