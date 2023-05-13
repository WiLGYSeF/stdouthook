using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class ActivationRuleTest
{
    [Fact]
    public void NotEnabled()
    {
        var rule = new TestRule
        {
            Enabled = false,
        };

        var state = new ProfileState();
        rule.Build(state, null!);

        SendLine(rule, "test", true, state).ShouldBe(false);
    }

    [Fact]
    public void StdoutOnly()
    {
        var rule = new TestRule
        {
            StdoutOnly = true,
        };

        var state = new ProfileState();
        rule.Build(state, null!);

        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", false, state).ShouldBe(false);
    }

    [Fact]
    public void StderrOnly()
    {
        var rule = new TestRule
        {
            StderrOnly = true,
        };

        var state = new ProfileState();
        rule.Build(state, null!);

        SendLine(rule, "test", true, state).ShouldBe(false);
        SendLine(rule, "test", false, state).ShouldBe(true);
    }

    [Fact]
    public void ActivationLines()
    {
        var rule = new TestRule
        {
            ActivationLines = new List<long> { 5 },
            DeactivationLines = new List<long> { 3 },
        };

        var state = new ProfileState();
        rule.Build(state, null!);

        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(false);
        SendLine(rule, "test", true, state).ShouldBe(false);
        SendLine(rule, "test", true, state).ShouldBe(true);
    }

    [Fact]
    public void ActivationLines_StdoutOnly()
    {
        var rule = new TestRule
        {
            ActivationLinesStdoutOnly = new List<long> { 5 },
            DeactivationLinesStdoutOnly = new List<long> { 3 },
        };

        var state = new ProfileState();
        rule.Build(state, null!);

        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", false, state).ShouldBe(true);
        SendLine(rule, "test", false, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(false);
        SendLine(rule, "test", false, state).ShouldBe(false);
        SendLine(rule, "test", true, state).ShouldBe(false);
        SendLine(rule, "test", true, state).ShouldBe(true);
    }

    [Fact]
    public void ActivationLines_StderrOnly()
    {
        var rule = new TestRule
        {
            ActivationLinesStderrOnly = new List<long> { 5 },
            DeactivationLinesStderrOnly = new List<long> { 3 },
        };

        var state = new ProfileState();
        rule.Build(state, null!);

        SendLine(rule, "test", false, state).ShouldBe(true);
        SendLine(rule, "test", false, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", false, state).ShouldBe(false);
        SendLine(rule, "test", true, state).ShouldBe(false);
        SendLine(rule, "test", false, state).ShouldBe(false);
        SendLine(rule, "test", false, state).ShouldBe(true);
    }

    [Fact]
    public void ActivationLines_BothActivateAndDeactivate()
    {
        var rule = new TestRule
        {
            ActivationLinesStderrOnly = new List<long> { 3 },
            DeactivationLinesStderrOnly = new List<long> { 3 },
        };

        var state = new ProfileState();
        rule.Build(state, null!);

        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(true);
    }

    [Fact]
    public void ActivationLines_Multiple()
    {
        var rule = new TestRule
        {
            ActivationLines = new List<long> { 7, 5, 5 },
            DeactivationLines = new List<long> { 3, 6, 3 },
        };

        var state = new ProfileState();
        rule.Build(state, null!);

        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(false);
        SendLine(rule, "test", true, state).ShouldBe(false);
        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(false);
        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(true);
    }

    [Fact]
    public void EnableRegex()
    {
        var rule = new TestRule
        {
            EnableRegex = new Regex(@"abc"),
        };

        var state = new ProfileState();
        rule.Build(state, null!);

        SendLine(rule, "test", true, state).ShouldBe(false);
        SendLine(rule, "abc", true, state).ShouldBe(true);
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

        var state = new ProfileState();
        rule.Build(state, null!);

        SendLine(rule, "abc", true, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(true);
        SendLine(rule, "test", true, state).ShouldBe(false);
        SendLine(rule, "test", true, state).ShouldBe(false);
    }

    private static bool SendLine(Rule rule, string data, bool stdout, ProfileState profileState)
    {
        if (stdout)
        {
            profileState.StdoutLineCount++;
        }
        else
        {
            profileState.StderrLineCount++;
        }

        return rule.IsActive(new DataState(data, stdout, profileState));
    }

    private class TestRule : Rule
    {
        internal override string Apply(DataState state)
        {
            throw new NotImplementedException();
        }
    }
}
