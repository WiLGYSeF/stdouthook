namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class FormatTest : RuleTestBase
{
    [Fact]
    public void Literal()
    {
        var formatter = GetFormatter();
        var compiledFormat = formatter.CompileFormat("test");

        compiledFormat.ToString().ShouldBe("test");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Escaped()
    {
        var formatter = GetFormatter();
        var compiledFormat = formatter.CompileFormat("%%test");

        compiledFormat.ToString().ShouldBe("%test");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseCharEof()
    {
        var formatter = GetFormatter();
        var compiledFormat = formatter.CompileFormat("test%");

        compiledFormat.ToString().ShouldBe("test%");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Single()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C', _ => "asdf"));
        var compiledFormat = formatter.CompileFormat("%C");

        compiledFormat.ToString().ShouldBe("asdf");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("asdf");
    }

    [Fact]
    public void Single_CaseSensitive()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C', _ => "asdf"));
        var compiledFormat = formatter.CompileFormat("%c");

        compiledFormat.ToString().ShouldBe("%c");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Single_InParentheses()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C', _ => "asdf"));
        var compiledFormat = formatter.CompileFormat("abc%(C)def");

        compiledFormat.ToString().ShouldBe("abcasdfdef");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("asdf");
    }

    [Fact]
    public void Single_Param()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C'));
        var compiledFormat = formatter.CompileFormat("%Cabc");

        compiledFormat.ToString().ShouldBe("abc");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("abc");
    }

    [Fact]
    public void Single_ParamParentheses()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C'));
        var compiledFormat = formatter.CompileFormat("%C(abc)");

        compiledFormat.ToString().ShouldBe("abc");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("abc");
    }

    [Fact]
    public void Single_Param_InParentheses()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C'));
        var compiledFormat = formatter.CompileFormat("abc%(Cabc)def");

        compiledFormat.ToString().ShouldBe("abcabcdef");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("abc");
    }

    [Fact]
    public void Format()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "asdf"));
        var compiledFormat = formatter.CompileFormat("%test");

        compiledFormat.ToString().ShouldBe("asdf");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("asdf");
    }

    [Fact]
    public void CaseInsensitive()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "asdf"));
        var compiledFormat = formatter.CompileFormat("%Test");

        compiledFormat.ToString().ShouldBe("asdf");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("asdf");
    }

    [Fact]
    public void Param_Colon()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null));
        var compiledFormat = formatter.CompileFormat("%(test:a)");

        compiledFormat.ToString().ShouldBe("a");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("a");
    }

    [Fact]
    public void NotParam()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "abc"));
        var compiledFormat = formatter.CompileFormat("%test(asdf)");

        compiledFormat.ToString().ShouldBe("abc(asdf)");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("abc");
    }

    [Fact]
    public void EmptyNotParam()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "abc"));
        var compiledFormat = formatter.CompileFormat("%test()");

        compiledFormat.ToString().ShouldBe("abc()");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("abc");
    }

    [Fact]
    public void Constant()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "asdf", _ => true));
        var compiledFormat = formatter.CompileFormat("%test");

        compiledFormat.ToString().ShouldBe("asdf");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Multiple()
    {
        var formatter = GetFormatter(
            new TestFormatBuilder("test", null, _ => "asdf"),
            new TestFormatBuilder(null, 'C'));
        var compiledFormat = formatter.CompileFormat("this is a%Cb %test");

        compiledFormat.ToString().ShouldBe("this is ab asdf");
        compiledFormat.Parts.Length.ShouldBe(3);
        compiledFormat.Funcs.Length.ShouldBe(2);

        compiledFormat.Funcs[0]().ShouldBe("b");
        compiledFormat.Funcs[1]().ShouldBe("asdf");
    }

    [Fact]
    public void Unknown()
    {
        var formatter = GetFormatter();
        var compiledFormat = formatter.CompileFormat("this is a %test");

        compiledFormat.ToString().ShouldBe("this is a %test");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Unknown_Blank()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;

        var compiledFormat = formatter.CompileFormat("this is a %test");

        compiledFormat.ToString().ShouldBe("this is a ");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Invalid()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;

        var compiledFormat = formatter.CompileFormat("this is a %.");

        compiledFormat.ToString().ShouldBe("this is a %.");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Invalid_WithParam()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;

        var compiledFormat = formatter.CompileFormat("this is a %.(a)");

        compiledFormat.ToString().ShouldBe("this is a %.(a)");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Invalid_InParentheses()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;

        var compiledFormat = formatter.CompileFormat("this is a %(.)");

        compiledFormat.ToString().ShouldBe("this is a %(.)");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Parentheses_NonTerminal()
    {
        var formatter = GetFormatter();
        var compiledFormat = formatter.CompileFormat("%(test");

        compiledFormat.ToString().ShouldBe("%(test");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Parentheses_Nested()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null));
        var compiledFormat = formatter.CompileFormat("%(test:(a))");

        compiledFormat.ToString().ShouldBe("(a)");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("(a)");
    }
}
