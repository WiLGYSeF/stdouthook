using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class UnconditionalReplaceRuleTest : RuleTestBase
{
    [Fact]
    public void UnconditionalRule()
    {
        var rule = new UnconditionalReplaceRule
        {
            Format = "asdf %test abc",
        };
        var state = new ProfileState();

        rule.Build(state, GetFormatter(new TestFormatBuilder("test", null, _ => "123")));
        rule.Apply(CreateDataState("input")).ShouldBe("asdf 123 abc");
    }

    private static DataState CreateDataState(string data)
    {
        return new DataState(data, true, new ProfileState());
    }
}
