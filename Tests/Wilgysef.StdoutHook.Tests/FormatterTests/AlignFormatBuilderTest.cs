using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class AlignFormatBuilderTest : RuleTestBase
{
    [Fact]
    public void Align()
    {
        ShouldFormatBe("6:test", "test  ", " test ", "  test");
    }

    [Fact]
    public void OddLength()
    {
        ShouldFormatBe("7:test", "test   ", " test  ", "   test");
    }

    [Fact]
    public void Char()
    {
        ShouldFormatBe("_:6:test", "test__", "_test_", "__test");
    }

    [Fact]
    public void Char_Separator()
    {
        ShouldFormatBe("::6:test", "test::", ":test:", "::test");
    }

    [Fact]
    public void Shorter()
    {
        ShouldFormatBe("3:test", "test");
    }

    [Fact]
    public void Empty()
    {
        ShouldFormatBe("3:", "   ");
    }

    [Fact]
    public void Negative()
    {
        ShouldFormatBe("-5:test", "test");
    }

    [Fact]
    public void Invalid()
    {
        ShouldFormatBeInvalid(":6:test");
        ShouldFormatBeInvalid("::6a:test");
        ShouldFormatBeInvalid("::6");
        ShouldFormatBeInvalid("test");
        ShouldFormatBeInvalid("5");
    }

    private static void ShouldFormatBe(string contents, string expected)
    {
        ShouldFormatBe(contents, expected, expected, expected);
    }

    private static void ShouldFormatBe(string contents, string expectedLeft, string expectedCenter, string expectedRight)
    {
        ShouldFormatBe(Alignment.Left, contents, expectedLeft);
        ShouldFormatBe(Alignment.Center, contents, expectedCenter);
        ShouldFormatBe(Alignment.Right, contents, expectedRight);
    }

    private static void ShouldFormatBeInvalid(string contents)
    {
        ShouldFormatBe(contents, $"%(alignLeft:{contents})", $"%(alignCenter:{contents})", $"%(alignRight:{contents})");
    }

    private static void ShouldFormatBe(Alignment alignment, string contents, string expected)
    {
        using var profile = new Profile();
        var formatter = GetFormatter(alignment switch
        {
            Alignment.Left => new AlignLeftFormatBuilder(),
            Alignment.Center => new AlignCenterFormatBuilder(),
            Alignment.Right => new AlignRightFormatBuilder(),
            _ => throw new ArgumentOutOfRangeException(nameof(alignment)),
        });

        profile.Build(formatter);

        formatter.Format($"%(align{alignment}:{contents})", new DataState(profile)).ShouldBe(expected);
    }

#pragma warning disable SA1201 // Elements should appear in the correct order
    private enum Alignment
#pragma warning restore SA1201 // Elements should appear in the correct order
    {
        Left,
        Center,
        Right,
    }
}
