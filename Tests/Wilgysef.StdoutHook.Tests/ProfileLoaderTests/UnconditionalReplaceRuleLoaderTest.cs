using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.ProfileLoaderTests;

public class UnconditionalReplaceRuleLoaderTest
{
    [Fact]
    public void UnconditionalReplace()
    {
        var loader = new RuleLoader();
        var rule = (UnconditionalReplaceRule)loader.LoadRule(new RuleDto
        {
            ReplaceAllFormat = "a",
        });

        rule.Format.ShouldBe("a");
    }
}
