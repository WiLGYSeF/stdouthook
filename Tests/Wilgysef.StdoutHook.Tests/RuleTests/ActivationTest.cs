using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class ActivationTest
{
    [Fact]
    public void NotEnabled()
    {
        var rule = new TestRule
        {
            Enabled = false,
        };

        using var profile = CreateDummyProfile();
        rule.Build(profile, null!);

        SendLine(rule, "test", true, profile).ShouldBe(false);
    }

    [Fact]
    public void StdoutOnly()
    {
        var rule = new TestRule
        {
            StdoutOnly = true,
        };

        using var profile = CreateDummyProfile();
        rule.Build(profile, null!);

        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", false, profile).ShouldBe(false);
    }

    [Fact]
    public void StderrOnly()
    {
        var rule = new TestRule
        {
            StderrOnly = true,
        };

        using var profile = CreateDummyProfile();
        rule.Build(profile, null!);

        SendLine(rule, "test", true, profile).ShouldBe(false);
        SendLine(rule, "test", false, profile).ShouldBe(true);
    }

    [Fact]
    public void ActivationLines()
    {
        var rule = new TestRule
        {
            ActivationLines = new List<long> { 5 },
            DeactivationLines = new List<long> { 3 },
        };

        using var profile = CreateDummyProfile();
        rule.Build(profile, null!);

        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(false);
        SendLine(rule, "test", true, profile).ShouldBe(false);
        SendLine(rule, "test", true, profile).ShouldBe(true);
    }

    [Fact]
    public void ActivationLines_StdoutOnly()
    {
        var rule = new TestRule
        {
            ActivationLinesStdoutOnly = new List<long> { 5 },
            DeactivationLinesStdoutOnly = new List<long> { 3 },
        };

        using var profile = CreateDummyProfile();
        rule.Build(profile, null!);

        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", false, profile).ShouldBe(true);
        SendLine(rule, "test", false, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(false);
        SendLine(rule, "test", false, profile).ShouldBe(false);
        SendLine(rule, "test", true, profile).ShouldBe(false);
        SendLine(rule, "test", true, profile).ShouldBe(true);
    }

    [Fact]
    public void ActivationLines_StderrOnly()
    {
        var rule = new TestRule
        {
            ActivationLinesStderrOnly = new List<long> { 5 },
            DeactivationLinesStderrOnly = new List<long> { 3 },
        };

        using var profile = CreateDummyProfile();
        rule.Build(profile, null!);

        SendLine(rule, "test", false, profile).ShouldBe(true);
        SendLine(rule, "test", false, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", false, profile).ShouldBe(false);
        SendLine(rule, "test", true, profile).ShouldBe(false);
        SendLine(rule, "test", false, profile).ShouldBe(false);
        SendLine(rule, "test", false, profile).ShouldBe(true);
    }

    [Fact]
    public void ActivationLines_BothActivateAndDeactivate()
    {
        var rule = new TestRule
        {
            ActivationLinesStderrOnly = new List<long> { 3 },
            DeactivationLinesStderrOnly = new List<long> { 3 },
        };

        using var profile = CreateDummyProfile();
        rule.Build(profile, null!);

        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(true);
    }

    [Fact]
    public void ActivationLines_Multiple()
    {
        var rule = new TestRule
        {
            ActivationLines = new List<long> { 7, 5, 5 },
            DeactivationLines = new List<long> { 3, 6, 3 },
        };

        using var profile = CreateDummyProfile();
        rule.Build(profile, null!);

        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(false);
        SendLine(rule, "test", true, profile).ShouldBe(false);
        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(false);
        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(true);
    }

    [Fact]
    public void EnableRegex()
    {
        var rule = new TestRule
        {
            EnableExpression = new Regex(@"abc"),
        };

        using var profile = CreateDummyProfile();
        rule.Build(profile, null!);

        SendLine(rule, "test", true, profile).ShouldBe(false);
        SendLine(rule, "abc", true, profile).ShouldBe(true);
    }

    [Fact]
    public void ActivationExpression()
    {
        var rule = new TestRule
        {
            DeactivationExpressions = new List<ActivationExpression>
            {
                new ActivationExpression(new Regex(@"abc"), 2),
            },
        };

        using var profile = CreateDummyProfile();
        rule.Build(profile, null!);

        SendLine(rule, "abc", true, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(true);
        SendLine(rule, "test", true, profile).ShouldBe(false);
        SendLine(rule, "test", true, profile).ShouldBe(false);
    }

    private static bool SendLine(Rule rule, string data, bool stdout, Profile profile)
    {
        if (stdout)
        {
            profile.State.StdoutLineCount++;
        }
        else
        {
            profile.State.StderrLineCount++;
        }

        return rule.IsActive(new DataState(data, stdout, profile));
    }

    private static Profile CreateDummyProfile()
    {
        return new Profile();
    }

    private class TestRule : Rule
    {
        internal override string Apply(DataState state)
        {
            throw new NotImplementedException();
        }
    }
}
