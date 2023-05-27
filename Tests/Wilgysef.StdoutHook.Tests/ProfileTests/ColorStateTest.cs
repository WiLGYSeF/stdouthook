using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Tests.ProfileTests;

public class ColorStateTest
{
    [Fact]
    public void PositionLimit()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[31m");
        AddColor(colors, 4, "\x1b[32m");

        state.UpdateState(colors, 2);
        state.ForegroundColor.ShouldBe("31");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Unknown()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[255m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void String()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[31;42;1;5m");

        state.UpdateState(colors, 1000);
        state.ToString().ShouldBe("\x1b[31;42;1;5m");
    }

    [Fact]
    public void NoColor()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "asdf");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Foreground()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[31m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("31");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Background()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[41m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("41");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Foreground_Background_Multiple()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[31;43m");
        AddColor(colors, 0, "\x1b[33m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("33");
        state.BackgroundColor.ShouldBe("43");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Foreground_8Bit()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[38;5;123m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("38;5;123");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Background_8Bit()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[48;5;123m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("48;5;123");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Reset()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[31;43;1m");
        AddColor(colors, 4, "\x1b[0m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Styles()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[1;6m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(2);
        state.Styles.Contains(1);
        state.Styles.Contains(6);
    }

    [Fact]
    public void BoldOff_DoubleUnderline()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[1;21m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(1);
        state.Styles.Contains(21);
    }

    [Fact]
    public void BlinkOff()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[5;25m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void NormalColor()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[1;2;22m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void ItalicOff()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[3;20;23m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void UnderlineOff()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[4;21;24m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void FrameEncircledOff()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[51;52;54m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void OverlineOff()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[53;55m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void IdeogramOff()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[60;61;62;63;64;65m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void ColorSet_Missing()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[38m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void ColorSet_Invalid()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[38;9m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);

        colors = new ColorList();
        AddColor(colors, 0, "\x1b[38;wm");
        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("39");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Color8Bit_Missing()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[38;5m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("38;5");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Color24Bit_ZeroNum()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[38;2m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("38;2");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Color24Bit_OneNum()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[38;2;1m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("38;2;1");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Color24Bit_TwoNum()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[38;2;1;2m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("38;2;1;2");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Color24Bit_ThreeNum()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[38;2;1;2;3m");

        state.UpdateState(colors, 1000);
        state.ForegroundColor.ShouldBe("38;2;1;2;3");
        state.BackgroundColor.ShouldBe("49");
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Copy()
    {
        var state = new ColorState();
        var colors = new ColorList();
        AddColor(colors, 0, "\x1b[31;42;1;5m");

        state.UpdateState(colors, 1000);
        var copy = state.Copy();
        copy.ForegroundColor.ShouldBe(state.ForegroundColor);
        copy.BackgroundColor.ShouldBe(state.BackgroundColor);
        copy.Styles.SequenceEqual(state.Styles).ShouldBeTrue();
    }

    private static void AddColor(ColorList colors, int position, string color)
    {
        colors.AddColor(position, color, 0, color.Length);
    }
}
