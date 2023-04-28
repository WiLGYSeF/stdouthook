using Wilgysef.StdoutHook.Formatters;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class FormatTest
{
    [Fact]
    public void A()
    {
        var formatter = GetFormatter();
        var compiledFormat = formatter.CompileFormat("test %(uiop) %asdf-");
    }

    private static Formatter GetFormatter()
    {
        return new Formatter(FormatFunctionBuilder.Create());
    }
}
