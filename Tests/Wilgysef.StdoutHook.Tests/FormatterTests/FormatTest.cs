using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class FormatTest : RuleTestBase
{
    [Fact]
    public void Literal()
    {
        var formatter = GetFormatter();
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("test", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("test");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Escaped()
    {
        var formatter = GetFormatter();
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("%%test", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("%test");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseCharEof()
    {
        var formatter = GetFormatter();
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("test%", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("test%");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Single()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C', _ => "asdf"));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("%C", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("asdf");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0](CreateDataState(state)).ShouldBe("asdf");
    }

    [Fact]
    public void Single_CaseSensitive()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C', _ => "asdf"));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("%c", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("%c");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Single_InParentheses()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C', _ => "asdf"));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("abc%(C)def", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("abcasdfdef");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0](CreateDataState(state)).ShouldBe("asdf");
    }

    [Fact]
    public void Single_Param()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C'));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("%Cabc", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("abc");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0](CreateDataState(state)).ShouldBe("abc");
    }

    [Fact]
    public void Single_ParamParentheses()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C'));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("%C(abc)", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("abc");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0](CreateDataState(state)).ShouldBe("abc");
    }

    [Fact]
    public void Single_Param_InParentheses()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C'));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("abc%(Cabc)def", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("abcabcdef");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0](CreateDataState(state)).ShouldBe("abc");
    }

    [Fact]
    public void Format()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "asdf"));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("%test", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("asdf");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0](CreateDataState(state)).ShouldBe("asdf");
    }

    [Fact]
    public void CaseInsensitive()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "asdf"));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("%Test", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("asdf");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0](CreateDataState(state)).ShouldBe("asdf");
    }

    [Fact]
    public void Param_Colon()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("%(test:a)", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("a");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0](CreateDataState(state)).ShouldBe("a");
    }

    [Fact]
    public void NotParam()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "abc"));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("%test(asdf)", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("abc(asdf)");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0](CreateDataState(state)).ShouldBe("abc");
    }

    [Fact]
    public void EmptyNotParam()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "abc"));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("%test()", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("abc()");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0](CreateDataState(state)).ShouldBe("abc");
    }

    [Fact]
    public void Constant()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "asdf", _ => true));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("%test", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("asdf");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Multiple()
    {
        var formatter = GetFormatter(
            new TestFormatBuilder("test", null, _ => "asdf"),
            new TestFormatBuilder(null, 'C'));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("this is a%Cb %test", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("this is ab asdf");
        compiledFormat.Parts.Length.ShouldBe(3);
        compiledFormat.Funcs.Length.ShouldBe(2);

        compiledFormat.Funcs[0](CreateDataState(state)).ShouldBe("b");
        compiledFormat.Funcs[1](CreateDataState(state)).ShouldBe("asdf");
    }

    [Fact]
    public void Unknown()
    {
        var formatter = GetFormatter();
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("this is a %test", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("this is a %test");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Unknown_Blank()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("this is a %test", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("this is a ");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Invalid()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("this is a %.", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("this is a %.");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Invalid_WithParam()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("this is a %.(a)", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("this is a %.(a)");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Invalid_InParentheses()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("this is a %(.)", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("this is a %(.)");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Parentheses_NonTerminal()
    {
        var formatter = GetFormatter();
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("%(test", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("%(test");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Parentheses_Nested()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null));
        var state = new ProfileState();

        var compiledFormat = formatter.CompileFormat("%(test:(a))", state);
        compiledFormat.Compute(CreateDataState(state)).ShouldBe("(a)");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0](CreateDataState(state)).ShouldBe("(a)");
    }

    private static DataState CreateDataState(ProfileState state)
    {
        return new DataState("", true, state);
    }
}
