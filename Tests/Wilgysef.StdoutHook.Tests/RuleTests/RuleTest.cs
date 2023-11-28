using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class RuleTest
{
    [Fact]
    public void Build_StdoutOnly_StderrOnly_Both()
    {
        var rule = new TestRule
        {
            StdoutOnly = true,
            StderrOnly = true,
        };

        Should.Throw<InvalidOperationException>(() => rule.Build(CreateDummyProfile(), null!));
    }

    [Fact]
    public void Copy()
    {
        var rule = new TestRule
        {
            EnableExpression = new Regex("enable"),
            Terminal = true,
            TrimNewline = true,
            ActivationLines = new List<long> { 1, 3, 5 },
            DeactivationLines = new List<long> { 2, 4, 6 },
        };

        rule.ActivationExpressions.Add(new ActivationExpression(new Regex("active")));
        rule.DeactivationExpressions.Add(new ActivationExpression(new Regex("deactive")));

        rule.ActivationLinesStdoutOnly = new List<long> { 7, 8, 9 };
        rule.DeactivationLinesStdoutOnly = new List<long> { 10, 11, 12 };

        rule.ActivationExpressionsStdoutOnly.Add(new ActivationExpression(new Regex("aeso")));
        rule.DeactivationExpressionsStdoutOnly.Add(new ActivationExpression(new Regex("deso")));

        rule.ActivationLinesStderrOnly = new List<long> { 13, 14, 15 };
        rule.DeactivationLinesStderrOnly = new List<long> { 16, 17, 18 };

        rule.ActivationExpressionsStderrOnly.Add(new ActivationExpression(new Regex("aese")));
        rule.DeactivationExpressionsStderrOnly.Add(new ActivationExpression(new Regex("dese")));

        var copy = rule.Copy();

        copy.EnableExpression!.ToString().ShouldBe("enable");
        copy.Terminal.ShouldBeTrue();
        copy.TrimNewline.ShouldBeTrue();
        copy.ActivationLines.SequenceEqual(rule.ActivationLines).ShouldBeTrue();
        copy.DeactivationLines.SequenceEqual(rule.DeactivationLines).ShouldBeTrue();

        copy.ActivationLines.SequenceEqual(rule.ActivationLines).ShouldBeTrue();
        copy.DeactivationLines.SequenceEqual(rule.DeactivationLines).ShouldBeTrue();

        copy.ActivationExpressions.ElementAt(0).Expression.ToString().ShouldBe("active");
        copy.DeactivationExpressions.ElementAt(0).Expression.ToString().ShouldBe("deactive");

        copy.ActivationLinesStdoutOnly.SequenceEqual(rule.ActivationLinesStdoutOnly).ShouldBeTrue();
        copy.DeactivationLinesStdoutOnly.SequenceEqual(rule.DeactivationLinesStdoutOnly).ShouldBeTrue();

        copy.ActivationExpressionsStdoutOnly.ElementAt(0).Expression.ToString().ShouldBe("aeso");
        copy.DeactivationExpressionsStdoutOnly.ElementAt(0).Expression.ToString().ShouldBe("deso");

        copy.ActivationLinesStderrOnly.SequenceEqual(rule.ActivationLinesStderrOnly).ShouldBeTrue();
        copy.DeactivationLinesStderrOnly.SequenceEqual(rule.DeactivationLinesStderrOnly).ShouldBeTrue();

        copy.ActivationExpressionsStderrOnly.ElementAt(0).Expression.ToString().ShouldBe("aese");
        copy.DeactivationExpressionsStderrOnly.ElementAt(0).Expression.ToString().ShouldBe("dese");
    }

    private static Profile CreateDummyProfile()
    {
        return new Profile();
    }

    private class TestRule : Rule
    {
        internal override string Apply(DataState state) => throw new NotImplementedException();

        protected override Rule CopyInternal() => new TestRule();
    }
}
