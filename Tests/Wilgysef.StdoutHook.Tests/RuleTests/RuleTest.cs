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

    private static Profile CreateDummyProfile()
    {
        return new Profile();
    }

    private class TestRule : Rule
    {
        internal override string Apply(DataState state) => throw new NotImplementedException();

        protected override Rule CopyInternal() => throw new NotImplementedException();
    }
}
