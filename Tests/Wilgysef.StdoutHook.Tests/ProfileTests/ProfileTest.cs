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

    [Fact]
    public void Split()
    {
        using var profile = new Profile
        {
            ProfileName = "test",
            Flush = true,
            BufferSize = 1024,
            OutputFlushInterval = 100,
            Interactive = true,
            InteractiveFlushInterval = 100,
            Rules = new List<Rule>
            {
                new UnconditionalReplaceRule("a"),
                new UnconditionalReplaceRule("b")
                {
                    StdoutOnly = true,
                },
                new UnconditionalReplaceRule("c")
                {
                    StderrOnly = true,
                },
            },
            CustomColors = new Dictionary<string, string>
            {
                ["a"] = "b",
            },
        };

        profile.Split(out var stdoutProfile, out var stderrProfile);

        stdoutProfile.ProfileName.ShouldBe(profile.ProfileName);
        stdoutProfile.Flush.ShouldBeTrue();
        stdoutProfile.BufferSize.ShouldBe(profile.BufferSize);
        stdoutProfile.OutputFlushInterval.ShouldBe(profile.OutputFlushInterval);
        stdoutProfile.Interactive.ShouldBe(profile.Interactive);
        stdoutProfile.InteractiveFlushInterval.ShouldBe(profile.InteractiveFlushInterval);

        stdoutProfile.Rules.Count.ShouldBe(2);
        ((UnconditionalReplaceRule)stdoutProfile.Rules[0]).Format.ShouldBe("a");
        ((UnconditionalReplaceRule)stdoutProfile.Rules[1]).Format.ShouldBe("b");

        stdoutProfile.CustomColors.Count.ShouldBe(1);
        stdoutProfile.CustomColors["a"].ShouldBe("b");

        stderrProfile.ProfileName.ShouldBe(profile.ProfileName);
        stderrProfile.Flush.ShouldBeTrue();
        stderrProfile.BufferSize.ShouldBe(profile.BufferSize);
        stderrProfile.OutputFlushInterval.ShouldBe(profile.OutputFlushInterval);
        stderrProfile.Interactive.ShouldBe(profile.Interactive);
        stderrProfile.InteractiveFlushInterval.ShouldBe(profile.InteractiveFlushInterval);

        stderrProfile.Rules.Count.ShouldBe(2);
        ((UnconditionalReplaceRule)stderrProfile.Rules[0]).Format.ShouldBe("a");
        ((UnconditionalReplaceRule)stderrProfile.Rules[1]).Format.ShouldBe("c");

        stderrProfile.CustomColors.Count.ShouldBe(1);
        stderrProfile.CustomColors["a"].ShouldBe("b");

        profile.State.StdoutLineCount++;
        stdoutProfile.State.StdoutLineCount.ShouldBe(1);
        stderrProfile.State.StdoutLineCount.ShouldBe(1);

        stdoutProfile.Dispose();
        stderrProfile.Dispose();
    }
}
