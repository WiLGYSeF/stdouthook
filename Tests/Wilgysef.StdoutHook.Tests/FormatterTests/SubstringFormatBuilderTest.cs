using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class SubstringFormatBuilderTest : RuleTestBase
{
    [Fact]
    public void Substring_Start()
    {
        ShouldFormatBe("%(substring:2:test)", "st");
    }

    [Fact]
    public void Substring_Start_Zero()
    {
        ShouldFormatBe("%(substring:0:test)", "test");
    }

    [Fact]
    public void Substring_Start_Exceed()
    {
        ShouldFormatBe("%(substring:4:test)", "");
    }

    [Fact]
    public void Substring_Start_Negative()
    {
        ShouldFormatBe("%(substring:-1:test)", "t");
        ShouldFormatBe("%(substring:-2:test)", "st");
    }

    [Fact]
    public void Substring_Start_InvalidIndex()
    {
        ShouldFormatBe("%(substring:a:test)", "%(substring:a:test)");
    }

    [Fact]
    public void Substring_Invalid()
    {
        ShouldFormatBe("%(substring)", "%(substring)");
        ShouldFormatBe("%(substring:1)", "%(substring:1)");
        ShouldFormatBe("%(substring:a)", "%(substring:a)");
    }

    [Fact]
    public void Substring_End()
    {
        ShouldFormatBe("%(substring:0:2:test)", "te");
    }

    [Fact]
    public void Substring_End_Zero()
    {
        ShouldFormatBe("%(substring:0:0:test)", "");
    }

    [Fact]
    public void Substring_End_Exceed()
    {
        ShouldFormatBe("%(substring:1:6:test)", "est");
    }

    [Fact]
    public void Substring_End_Negative()
    {
        ShouldFormatBe("%(substring:1:-1:test)", "es");
        ShouldFormatBe("%(substring:1:-2:test)", "e");
    }

    [Fact]
    public void Substring_End_InvalidIndex()
    {
        ShouldFormatBe("%(substring:1:3a:test)", "a:test");
    }

    private static void ShouldFormatBe(string format, string expected)
    {
        using var profile = new Profile();
        var formatter = GetFormatter(new SubstringFormatBuilder());

        profile.Build(formatter);

        formatter.Format(format, new DataState(profile)).ShouldBe(expected);
    }
}
