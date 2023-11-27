using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.ProfileLoaderTests;

public class TeeRuleLoaderTest
{
    [Fact]
    public void Tee()
    {
        var loader = new RuleLoader();
        var rule = (TeeRule)loader.LoadRule(new RuleDto
        {
            Filename = "a",
        });

        rule.Filename.ShouldBe("a");
    }

    [Fact]
    public void Tee_Flush()
    {
        var loader = new RuleLoader();
        var rule = (TeeRule)loader.LoadRule(new RuleDto
        {
            Filename = "a",
            Flush = true,
        });

        rule.Filename.ShouldBe("a");
        rule.Flush.ShouldBeTrue();
    }

    [Fact]
    public void Tee_ExtractColors()
    {
        var loader = new RuleLoader();
        var rule = (TeeRule)loader.LoadRule(new RuleDto
        {
            Filename = "a",
            ExtractColors = true,
        });

        rule.Filename.ShouldBe("a");
        rule.ExtractColors.ShouldBeTrue();
    }
}
