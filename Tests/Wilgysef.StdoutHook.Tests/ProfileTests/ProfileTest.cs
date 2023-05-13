using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.ProfileTests;

public class ProfileTest
{
    [Fact]
    public void FilterLine()
    {
        var profile = new Profile(new ProfileState())
        {
            Rules = new List<Rule>
            {
                new FilterRule()
            }
        };

        profile.Build();

        string line = "";
        profile.ApplyRules(ref line, true).ShouldBeFalse();
    }

    [Fact]
    public void Terminal()
    {
        var profile = new Profile(new ProfileState())
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

        string line = "";
        profile.ApplyRules(ref line, true);

        line.ShouldBe("asdf");
    }

    [Fact]
    public void CustomColors()
    {
        var profile = new Profile(new ProfileState())
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

        string line = "";
        profile.ApplyRules(ref line, true);

        line.ShouldBe("\x1b[31m123");
    }
}
