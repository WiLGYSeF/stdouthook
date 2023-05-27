using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.ProfileTests;

public class ProfileTest
{
    [Fact]
    public void FilterLine()
    {
        using var profile = new Profile
        {
            Rules = new List<Rule>
            {
                new FilterRule()
            }
        };

        profile.Build();

        profile.ApplyRules("", true).ShouldBeNull();
    }

    [Fact]
    public void Terminal()
    {
        using var profile = new Profile
        {
            Rules = new List<Rule>
            {
                new UnconditionalReplaceRule("asdf")
                {
                    Terminal = true,
                },
                new UnconditionalReplaceRule("test"),
            }
        };

        profile.Build();

        profile.ApplyRules("", true).ShouldBe("asdf");
    }

    [Fact]
    public void CustomColors()
    {
        using var profile = new Profile
        {
            CustomColors = new Dictionary<string, string>
            {
                ["test"] = "red",
            },
            Rules = new List<Rule>
            {
                new UnconditionalReplaceRule("%C(test)123")
            }
        };

        profile.Build();

        profile.ApplyRules("", true).ShouldBe("\x1b[31m123");
    }
}
