using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.ProfileLoaderTests;

public class FilterRuleLoaderTest
{
    [Fact]
    public void Filter()
    {
        var loader = new RuleLoader();
        _ = (FilterRule)loader.LoadRule(new RuleDto
        {
            Filter = true,
        });
    }
}
