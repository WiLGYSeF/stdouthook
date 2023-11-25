using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.ProfileLoaderTests;

public class RegexGroupRuleLoaderTest
{
    [Fact]
    public void RegexGroup_ReplaceGroups_List()
    {
        var loader = new RuleLoader();
        var rule = (RegexGroupRule)loader.LoadRule(new RuleDto
        {
            Regex = "test",
            ReplaceGroups = new List<object?> { "a", "b", "c" },
        });

        rule.Regex.ToString().ShouldBe("test");

        rule.ReplaceGroups!.Count.ShouldBe(3);
    }

    [Fact]
    public void RegexGroup_ReplaceGroups_Object()
    {
        var loader = new RuleLoader();
        var rule = (RegexGroupRule)loader.LoadRule(new RuleDto
        {
            Regex = "test",
            ReplaceGroups = new Dictionary<string, object?>
            {
                ["1"] = "a",
                ["group"] = "b",
            },
        });

        rule.Regex.ToString().ShouldBe("test");

        rule.ReplaceGroups!.Count.ShouldBe(1);
        rule.ReplaceGroups[0].Key.ToString().ShouldBe("1");
        rule.ReplaceGroups[0].Value.ShouldBe("a");

        rule.ReplaceNamedGroups!.Count.ShouldBe(1);
        rule.ReplaceNamedGroups["group"].ShouldBe("b");
    }

    [Fact]
    public void RegexGroup_Invalid()
    {
        var loader = new RuleLoader();

        Should.Throw<InvalidRuleException>(() => loader.LoadRule(new RuleDto
        {
            Regex = "test",
        }));

        Should.Throw<InvalidRuleException>(() => loader.LoadRule(new RuleDto
        {
            Regex = "test",
            ReplaceGroups = new List<object?> { "a", 2 },
        }));

        Should.Throw<InvalidRuleException>(() => loader.LoadRule(new RuleDto
        {
            Regex = "test",
            ReplaceGroups = new Dictionary<object, object?>
            {
                ["a"] = "1",
                ["b"] = 2,
            },
        }));
    }
}
