using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class FormatTest : RuleTestBase
{
    [Fact]
    public void Literal()
    {
        var formatter = GetFormatter();
        ShouldFormatBe(formatter, "test", "test");
    }

    [Fact]
    public void Escaped()
    {
        var formatter = GetFormatter();
        ShouldFormatBe(formatter, "%%test", "%test");
    }

    [Fact]
    public void ParseCharEof()
    {
        var formatter = GetFormatter();
        ShouldFormatBe(formatter, "test%", "test%");
    }

    [Fact]
    public void Single()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C', _ => "asdf"));
        ShouldFormatBe(formatter, "%C", "asdf", "asdf");
    }

    [Fact]
    public void Single_CaseSensitive()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C', _ => "asdf"));
        ShouldFormatBe(formatter, "%c", "%c");
    }

    [Fact]
    public void Single_Consecutive()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C', _ => "asdf"));
        ShouldFormatBe(formatter, "%C%C", "asdfasdf", "asdf", "asdf");
    }

    [Fact]
    public void Single_InParentheses()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C', _ => "asdf"));
        ShouldFormatBe(formatter, "abc%(C)def", "abcasdfdef", "asdf");
    }

    [Fact]
    public void Single_Param()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C'));
        ShouldFormatBe(formatter, "%Cabc", "abc", "abc");
    }

    [Fact]
    public void Single_ParamParentheses()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C'));
        ShouldFormatBe(formatter, "%C(abc)", "abc", "abc");
    }

    [Fact]
    public void Single_Param_InParentheses()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C'));
        ShouldFormatBe(formatter, "abc%(Cabc)def", "abcabcdef", "abc");
    }

    [Fact]
    public void Format()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "asdf"));
        ShouldFormatBe(formatter, "%test", "asdf", "asdf");
    }

    [Fact]
    public void CaseInsensitive()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "asdf"));
        ShouldFormatBe(formatter, "%Test", "asdf", "asdf");
    }

    [Fact]
    public void Format_Consecutive()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "asdf"));
        ShouldFormatBe(formatter, "%test%test", "asdfasdf", "asdf", "asdf");
    }

    [Fact]
    public void Format_Consecutive_Param()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null));
        ShouldFormatBe(formatter, "%(test:abc)%(test:def)", "abcdef", "abc", "def");
    }

    [Fact]
    public void Param_Colon()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null));
        ShouldFormatBe(formatter, "%(test:a)", "a", "a");
    }

    [Fact]
    public void NotParam()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "abc"));
        ShouldFormatBe(formatter, "%test(asdf)", "abc(asdf)", "abc");
    }

    [Fact]
    public void EmptyNotParam()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "abc"));
        ShouldFormatBe(formatter, "%test()", "abc()", "abc");
    }

    [Fact]
    public void Constant()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "asdf", _ => true));
        ShouldFormatBe(formatter, "%test", "asdf");
    }

    [Fact]
    public void Multiple()
    {
        var formatter = GetFormatter(
            new TestFormatBuilder("test", null, _ => "asdf"),
            new TestFormatBuilder(null, 'C'));

        ShouldFormatBe(formatter, "this is a%Cb %test", "this is ab asdf", "b", "asdf");
    }

    [Fact]
    public void Unknown()
    {
        var formatter = GetFormatter();
        ShouldFormatBe(formatter, "this is a %test", "this is a %test");
    }

    [Fact]
    public void Unknown_Blank()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;

        ShouldFormatBe(formatter, "this is a %test", "this is a ");
    }

    [Fact]
    public void Invalid()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;

        ShouldFormatBe(formatter, "this is a %.", "this is a %.");
    }

    [Fact]
    public void Invalid_WithParam()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;

        ShouldFormatBe(formatter, "this is a %.(a)", "this is a %.(a)");
    }

    [Fact]
    public void Invalid_InParentheses()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;

        ShouldFormatBe(formatter, "this is a %(.)", "this is a %(.)");
    }

    [Fact]
    public void Parentheses_NonTerminal()
    {
        var formatter = GetFormatter();
        ShouldFormatBe(formatter, "%(test", "%(test");
    }

    [Fact]
    public void Parentheses_Nested()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null));
        ShouldFormatBe(formatter, "%(test:(a))", "(a)", "(a)");
    }

    private static void ShouldFormatBe(
        Formatter formatter,
        string format,
        string expected,
        params string[] expectedFuncs)
    {
        var profile = new Profile(new ProfileState());
        var dataState = new DataState("", true, profile);

        var compiledFormat = formatter.CompileFormat(format, profile);
        compiledFormat.Compute(dataState).ShouldBe(expected);
        compiledFormat.Parts.Length.ShouldBe(expectedFuncs.Length + 1);
        compiledFormat.Funcs.Length.ShouldBe(expectedFuncs.Length);

        for (var i = 0; i < expectedFuncs.Length; i++)
        {
            compiledFormat.Funcs[i](dataState).ShouldBe(expectedFuncs[i]);
        }
    }
}
