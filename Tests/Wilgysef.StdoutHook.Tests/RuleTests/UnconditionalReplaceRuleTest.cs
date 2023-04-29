using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class UnconditionalReplaceRuleTest : RuleTestBase
{
    [Fact]
    public void UnconditionalRule()
    {
        var rule = new UnconditionalReplaceRule();

        rule.Build(GetFormatter(new TestFormatBuilder("test", null, _ => "123")));
        rule.Apply("asdf %test abc", true, new ProfileState()).ShouldBe("asdf 123 abc");
    }
}
