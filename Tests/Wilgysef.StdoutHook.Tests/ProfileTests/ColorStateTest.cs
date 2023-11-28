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
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Unknown()
    {
        var state = CreateState("\x1b[255m");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void String()
    {
        var state = CreateState("\x1b[31;42;1;5m");
        state.ToString().ShouldBe("\x1b[31;42;1;5m");
    }

    [Fact]
    public void StringEmpty()
    {
        new ColorState().ToString().ShouldBe("");
    }

    [Fact]
    public void StringNoForeground()
    {
        var state = CreateState("\x1b[42m");
        state.ToString().ShouldBe("\x1b[42m");
    }

    [Fact]
    public void StringNoBackground()
    {
        var state = CreateState("\x1b[31m");
        state.ToString().ShouldBe("\x1b[31m");
    }

    [Fact]
    public void StringStyleOnly()
    {
        var state = CreateState("\x1b[21m");
        state.ToString().ShouldBe("\x1b[21m");
    }

    [Fact]
    public void NoColor()
    {
        var state = CreateState("asdf");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Foreground()
    {
        var state = CreateState("\x1b[31m");
        state.ForegroundColor.ShouldBe("31");
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Background()
    {
        var state = CreateState("\x1b[41m");
        state.ForegroundColor.ShouldBeNull();
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
        var state = CreateState("\x1b[38;5;123m");
        state.ForegroundColor.ShouldBe("38;5;123");
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Background_8Bit()
    {
        var state = CreateState("\x1b[48;5;123m");
        state.ForegroundColor.ShouldBeNull();
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
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Styles()
    {
        var state = CreateState("\x1b[1;6m");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(2);
        state.Styles.Contains(1);
        state.Styles.Contains(6);
    }

    [Fact]
    public void BoldOff_DoubleUnderline()
    {
        var state = CreateState("\x1b[1;21m");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(1);
        state.Styles.Contains(21);
    }

    [Fact]
    public void BlinkOff()
    {
        var state = CreateState("\x1b[5;25m");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void NormalColor()
    {
        var state = CreateState("\x1b[1;2;22m");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void ItalicOff()
    {
        var state = CreateState("\x1b[3;20;23m");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void UnderlineOff()
    {
        var state = CreateState("\x1b[4;21;24m");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void FrameEncircledOff()
    {
        var state = CreateState("\x1b[51;52;54m");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void OverlineOff()
    {
        var state = CreateState("\x1b[53;55m");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void IdeogramOff()
    {
        var state = CreateState("\x1b[60;61;62;63;64;65m");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void ColorSet_Missing()
    {
        var state = CreateState("\x1b[38m");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void ColorSet_Invalid()
    {
        var state = CreateState("\x1b[38;9m");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);

        state = CreateState("\x1b[38;9m");
        state.ForegroundColor.ShouldBeNull();
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Color8Bit_Missing()
    {
        var state = CreateState("\x1b[38;5m");
        state.ForegroundColor.ShouldBe("38;5");
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Color24Bit_ZeroNum()
    {
        var state = CreateState("\x1b[38;2m");
        state.ForegroundColor.ShouldBe("38;2");
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Color24Bit_OneNum()
    {
        var state = CreateState("\x1b[38;2;1m");
        state.ForegroundColor.ShouldBe("38;2;1");
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Color24Bit_TwoNum()
    {
        var state = CreateState("\x1b[38;2;1;2m");
        state.ForegroundColor.ShouldBe("38;2;1;2");
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Color24Bit_ThreeNum()
    {
        var state = CreateState("\x1b[38;2;1;2;3m");
        state.ForegroundColor.ShouldBe("38;2;1;2;3");
        state.BackgroundColor.ShouldBeNull();
        state.Styles.Count.ShouldBe(0);
    }

    [Fact]
    public void Copy()
    {
        var state = CreateState("\x1b[31;42;1;5m");
        var copy = state.Copy();
        copy.ForegroundColor.ShouldBe(state.ForegroundColor);
        copy.BackgroundColor.ShouldBe(state.BackgroundColor);
        copy.Styles.SequenceEqual(state.Styles).ShouldBeTrue();
    }

    [Fact]
    public void StateEquals()
    {
        ColorStateEqual("\x1b[31m", "\x1b[31m").ShouldBeTrue();
        ColorStateEqual("\x1b[31m", "\x1b[32m").ShouldBeFalse();

        ColorStateEqual("\x1b[41m", "\x1b[41m").ShouldBeTrue();
        ColorStateEqual("\x1b[41m", "\x1b[42m").ShouldBeFalse();

        ColorStateEqual("\x1b[31;41m", "\x1b[31;41m").ShouldBeTrue();
        ColorStateEqual("\x1b[31;41m", "\x1b[32;41m").ShouldBeFalse();
        ColorStateEqual("\x1b[31;41m", "\x1b[31;42m").ShouldBeFalse();

        ColorStateEqual("\x1b[1m", "\x1b[1m").ShouldBeTrue();
        ColorStateEqual("\x1b[1m", "\x1b[4m").ShouldBeFalse();

        ColorStateEqual("\x1b[31;41;1m", "\x1b[31;41;1m").ShouldBeTrue();
        ColorStateEqual("\x1b[31;41;1m", "\x1b[31;41m").ShouldBeFalse();

        static bool ColorStateEqual(string color1, string color2)
            => CreateState(color1).Equals(CreateState(color2));
    }

    [Fact]
    public void Diff()
    {
        DiffState("\x1b[31m", "\x1b[31m").ShouldBe("");
        DiffState("\x1b[31m", "\x1b[32m").ShouldBe("\x1b[32m");
        DiffState("\x1b[41m", "\x1b[42m").ShouldBe("\x1b[42m");
        DiffState("\x1b[41m", "\x1b[31m").ShouldBe("\x1b[31;49m");
        DiffState("\x1b[31m", "\x1b[41m").ShouldBe("\x1b[39;41m");

        DiffState("\x1b[1;2m", "\x1b[3;4m").ShouldBe("\x1b[21;22;3;4m");
        DiffState("\x1b[31;1m", "\x1b[32;1m").ShouldBe("\x1b[32m");

        DiffState("\x1b[31;1;2;3;20;51;53;60m", "\x1b[31m").ShouldBe("\x1b[21;22;23;54;55;65m");

        static string DiffState(string color1, string color2)
            => CreateState(color1).Diff(CreateState(color2)).ToString();
    }

    private static ColorState CreateState(string color)
    {
        var state = new ColorState();
        var colors = new ColorList();

        AddColor(colors, 0, color);
        state.UpdateState(colors, int.MaxValue);
        return state;
    }

    private static void AddColor(ColorList colors, int position, string color)
    {
        colors.AddColor(position, color, 0, color.Length);
    }
}
