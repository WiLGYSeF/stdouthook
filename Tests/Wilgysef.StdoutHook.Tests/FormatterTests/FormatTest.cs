using Shouldly;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Formatters.FormatBuilders;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class FormatTest
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
    public void FormatSimple_Single()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C', _ => "asdf"));
        var compiledFormat = formatter.CompileFormat("%C");

        compiledFormat.ToString().ShouldBe("asdf");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("asdf");
    }

    [Fact]
    public void FormatSimple_Single_InParentheses()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C', _ => "asdf"));
        var compiledFormat = formatter.CompileFormat("abc%(C)def");

        compiledFormat.ToString().ShouldBe("abcasdfdef");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("asdf");
    }

    [Fact]
    public void FormatSimple_Single_Param()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C'));
        var compiledFormat = formatter.CompileFormat("%Cabc");

        compiledFormat.ToString().ShouldBe("abc");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("abc");
    }

    [Fact]
    public void FormatSimple_Single_ParamParentheses()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C'));
        var compiledFormat = formatter.CompileFormat("%C(abc)");

        compiledFormat.ToString().ShouldBe("abc");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("abc");
    }

    [Fact]
    public void FormatSimple_Single_Param_InParentheses()
    {
        var formatter = GetFormatter(new TestFormatBuilder(null, 'C'));
        var compiledFormat = formatter.CompileFormat("abc%(Cabc)def");

        compiledFormat.ToString().ShouldBe("abcabcdef");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("abc");
    }

    [Fact]
    public void FormatSimple()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "asdf"));
        var compiledFormat = formatter.CompileFormat("%test");

        compiledFormat.ToString().ShouldBe("asdf");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("asdf");
    }

    [Fact]
    public void Format_Constant()
    {
        var formatter = GetFormatter(new TestFormatBuilder("test", null, _ => "asdf", _ => true));
        var compiledFormat = formatter.CompileFormat("%test");

        compiledFormat.ToString().ShouldBe("asdf");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Format_Multiple()
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
    public void Format_Unknown()
    {
        var formatter = GetFormatter();
        var compiledFormat = formatter.CompileFormat("this is a %test");

        compiledFormat.ToString().ShouldBe("this is a %test");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Format_Unknown_Blank()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;

        var compiledFormat = formatter.CompileFormat("this is a %test");

        compiledFormat.ToString().ShouldBe("this is a ");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Format_Invalid()
    {
        var formatter = GetFormatter();
        formatter.InvalidFormatBlank = true;

        var compiledFormat = formatter.CompileFormat("this is a %.");

        compiledFormat.ToString().ShouldBe("this is a %.");
        compiledFormat.Parts.Length.ShouldBe(1);
        compiledFormat.Funcs.Length.ShouldBe(0);
    }

    [Fact]
    public void Format_Invalid_InParentheses()
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
        var compiledFormat = formatter.CompileFormat("%(test(a))");

        compiledFormat.ToString().ShouldBe("(a)");
        compiledFormat.Parts.Length.ShouldBe(2);
        compiledFormat.Funcs.Length.ShouldBe(1);

        compiledFormat.Funcs[0]().ShouldBe("(a)");
    }

    private static Formatter GetFormatter(params FormatBuilder[] formatBuilders)
    {
        return new Formatter(new FormatFunctionBuilder(formatBuilders));
    }

    private class TestFormatBuilder : FormatBuilder
    {
        public override string? Key => _key;

        public override char? KeyShort => _keyShort;

        private readonly string? _key;
        private readonly char? _keyShort;
        private readonly Func<string, string>? _func;
        private readonly Func<string, bool>? _constantFunc;

        public TestFormatBuilder(
            string? key,
            char? keyShort,
            Func<string, string>? func = null,
            Func<string, bool>? constantFunc = null)
        {
            _key = key;
            _keyShort = keyShort;
            _func = func;
            _constantFunc = constantFunc;
        }

        public override Func<string> Build(string format, out bool isConstant)
        {
            isConstant = _constantFunc != null && _constantFunc(format);
            return _func != null
                ? () => _func(format)
                : () => format;
        }
    }
}
