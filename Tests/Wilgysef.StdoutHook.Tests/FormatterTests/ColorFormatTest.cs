using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class ColorFormatTest : RuleTestBase
{
    [Fact]
    public void Color()
    {
        ShouldFormatBe("%C(red)test", @"\x1b[31mtest");
    }

    [Fact]
    public void Color_Shortname()
    {
        ShouldFormatBe("%C(r)test", @"\x1b[31mtest");
    }

    [Fact]
    public void Color_Multiple()
    {
        ShouldFormatBe("%C(r;bold)test", @"\x1b[31;1mtest");
    }

    [Fact]
    public void Color_Background()
    {
        ShouldFormatBe("%C(^r)test", @"\x1b[41mtest");
    }

    [Fact]
    public void Color_ForegroundBackground()
    {
        ShouldFormatBe("%C(g;^r)test", @"\x1b[32;41mtest");
    }

    [Fact]
    public void Style()
    {
        ShouldFormatBe("%C(italic)test", @"\x1b[3mtest");
    }

    [Fact]
    public void Style_Off()
    {
        ShouldFormatBe("%C(^italic)test", @"\x1b[23mtest");
    }

    [Fact]
    public void Style_Overline()
    {
        ShouldFormatBe("%C(overline)test%C(^overline)", @"\x1b[53mtest\x1b[55m");
    }

    [Fact]
    public void Color_Unknown()
    {
        ShouldFormatBe("%C(aaaa)test", "test");
    }

    [Fact]
    public void Color_Empty()
    {
        ShouldFormatBe("%C()test", "test");
    }

    private static void ShouldFormatBe(string format, string expected)
    {
        GetFormatter(new ColorFormatBuilder())
            .Format(format, new DataState(new ProfileState()))
            .ShouldBe(expected);
    }
}
