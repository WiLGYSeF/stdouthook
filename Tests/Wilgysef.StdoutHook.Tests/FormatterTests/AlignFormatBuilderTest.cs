using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class AlignFormatBuilderTest : RuleTestBase
{
    [Fact]
    public void Align()
    {
        ShouldFormatBe("%(align:6:test)", "  test");
    }

    [Fact]
    public void Char()
    {
        ShouldFormatBe("%(align:_:6:test)", "__test");
    }

    [Fact]
    public void Char_Separator()
    {
        ShouldFormatBe("%(align:::6:test)", "::test");
        ShouldFormatBe("%(align::6:test)", "::test");
    }

    [Fact]
    public void Shorter()
    {
        ShouldFormatBe("%(align:3:test)", "test");
    }

    [Fact]
    public void Empty()
    {
        ShouldFormatBe("%(align:3:)", "   ");
    }

    [Fact]
    public void Negative()
    {
        ShouldFormatBe("%(align:-5:test)", "test");
    }

    [Fact]
    public void Invalid()
    {
        ShouldFormatBe("%(align:test)", "%(align:test)");
        ShouldFormatBe("%(align:5)", "%(align:5)");
    }

    private static void ShouldFormatBe(string format, string expected)
    {
        using var profile = new Profile();
        var formatter = GetFormatter(new AlignFormatBuilder());

        profile.Build(formatter);

        formatter.Format(format, new DataState(profile)).ShouldBe(expected);
    }
}
