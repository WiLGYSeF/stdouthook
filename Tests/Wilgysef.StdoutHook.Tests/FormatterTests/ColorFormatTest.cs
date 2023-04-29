using Wilgysef.StdoutHook.Formatters.FormatBuilders;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class ColorFormatTest : RuleTestBase
{
    [Fact]
    public void Color()
    {
        var formatter = GetFormatter(new ColorFormatBuilder());
        formatter.Format("%C(red)test").ShouldBe(@"\x1b[31mtest");
    }

    [Fact]
    public void Color_Shortname()
    {
        var formatter = GetFormatter(new ColorFormatBuilder());
        formatter.Format("%C(r)test").ShouldBe(@"\x1b[31mtest");
    }

    [Fact]
    public void Color_Multiple()
    {
        var formatter = GetFormatter(new ColorFormatBuilder());
        formatter.Format("%C(r;bold)test").ShouldBe(@"\x1b[31;1mtest");
    }

    [Fact]
    public void Color_Background()
    {
        var formatter = GetFormatter(new ColorFormatBuilder());
        formatter.Format("%C(^r)test").ShouldBe(@"\x1b[41mtest");
    }

    [Fact]
    public void Color_ForegroundBackground()
    {
        var formatter = GetFormatter(new ColorFormatBuilder());
        formatter.Format("%C(g;^r)test").ShouldBe(@"\x1b[32;41mtest");
    }

    [Fact]
    public void Style()
    {
        var formatter = GetFormatter(new ColorFormatBuilder());
        formatter.Format("%C(italic)test").ShouldBe(@"\x1b[3mtest");
    }

    [Fact]
    public void Style_Off()
    {
        var formatter = GetFormatter(new ColorFormatBuilder());
        formatter.Format("%C(^italic)test").ShouldBe(@"\x1b[23mtest");
    }

    [Fact]
    public void Style_Overline()
    {
        var formatter = GetFormatter(new ColorFormatBuilder());
        formatter.Format("%C(overline)test%C(^overline)").ShouldBe(@"\x1b[53mtest\x1b[55m");
    }

    [Fact]
    public void Color_Unknown()
    {
        var formatter = GetFormatter(new ColorFormatBuilder());
        formatter.Format("%C(aaaa)test").ShouldBe("test");
    }

    [Fact]
    public void Color_Empty()
    {
        var formatter = GetFormatter(new ColorFormatBuilder());
        formatter.Format("%C()test").ShouldBe("test");
    }
}
