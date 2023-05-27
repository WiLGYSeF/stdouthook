using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class UnconditionalReplaceRuleTest : RuleTestBase
{
    [Fact]
    public void UnconditionalRule()
    {
        var rule = new UnconditionalReplaceRule("asdf %test abc");

        ShouldRuleBe(
            rule,
            GetFormatter(new TestFormatBuilder("test", null, _ => "123")),
            "input",
            "asdf 123 abc");
    }

    [Fact]
    public void Newline()
    {
        var rule = new UnconditionalReplaceRule("asdf %test abc");

        ShouldRuleBe(
            rule,
            GetFormatter(new TestFormatBuilder("test", null, _ => "123")),
            "input\n",
            "asdf 123 abc\n");
    }

    [Fact]
    public void TrimNewline()
    {
        var rule = new UnconditionalReplaceRule("a")
        {
            TrimNewline = true,
        };

        ShouldRuleBe(
            rule,
            GetFormatter(new TestFormatBuilder("test", null, _ => "123")),
            "input\n",
            "a");
    }

    [Fact]
    public void Copy()
    {
        var rule = new UnconditionalReplaceRule("a");
        var copy = (UnconditionalReplaceRule)rule.Copy();

        copy.Format.ShouldBe(rule.Format);
    }
}
