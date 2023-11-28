using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class ColorFormatTest : RuleTestBase
{
    [Fact]
    public void Color()
    {
        ShouldFormatBe("%C(red)test", "\x1b[31mtest");
    }

    [Fact]
    public void Color_Shortname()
    {
        ShouldFormatBe("%C(r)test", "\x1b[31mtest");
    }

    [Fact]
    public void Color_Multiple()
    {
        ShouldFormatBe("%C(r;bold)test", "\x1b[31;1mtest");
    }

    [Fact]
    public void Color_Background()
    {
        ShouldFormatBe("%C(^r)test", "\x1b[41mtest");
    }

    [Fact]
    public void Color_ForegroundBackground()
    {
        ShouldFormatBe("%C(g;^r)test", "\x1b[32;41mtest");
    }

    [Fact]
    public void Color_Raw()
    {
        ShouldFormatBe("%C(raw1;2;3)test", "\x1b[1;2;3mtest");
    }

    [Fact]
    public void Color_Int()
    {
        ShouldFormatBe("%C(123)test", "\x1b[38;5;123mtest");
    }

    [Fact]
    public void Color_Int_Invalid()
    {
        ShouldFormatBe("%C(256)test", @"test");
    }

    [Fact]
    public void Color_Int_Background()
    {
        ShouldFormatBe("%C(^123)test", "\x1b[48;5;123mtest");
    }

    [Fact]
    public void Color_Int_Multiple()
    {
        ShouldFormatBe("%C(bold;123)test", "\x1b[1;38;5;123mtest");
    }

    [Fact]
    public void Color_Hex()
    {
        ShouldFormatBe("%C(ffa600)test", "\x1b[38;2;255;166;0mtest");
        ShouldFormatBe("%C(0xffa600)test", "\x1b[38;2;255;166;0mtest");
        ShouldFormatBe("%C(^0xffa600)test", "\x1b[48;2;255;166;0mtest");

        ShouldFormatBe("%C(0xaaa)test", "test");
        ShouldFormatBe("%C(zxcvbn)test", "test");
    }

    [Fact]
    public void Color_Custom()
    {
        var colorFormatBuilder = new ColorFormatBuilder();
        colorFormatBuilder.CustomColors.Add("custom", "italic");
        colorFormatBuilder.CustomColors.Add("customMulti", "bold;123;");

        GetFormatter(colorFormatBuilder)
            .Format("%C(customMulti)test%C(custom)abc", CreateDummyDataState())
            .ShouldBe("\x1b[1;38;5;123mtest\x1b[3mabc");
    }

    [Fact]
    public void Color_CustomNamed()
    {
        var colorFormatBuilder = new ColorFormatBuilder();
        GetFormatter(colorFormatBuilder)
            .Format("%C(orange)test", CreateDummyDataState())
            .ShouldBe("\x1b[38;2;255;165;0mtest");
    }

    [Fact]
    public void Style()
    {
        ShouldFormatBe("%C(italic)test", "\x1b[3mtest");
    }

    [Fact]
    public void Style_Off()
    {
        ShouldFormatBe("%C(^italic)test", "\x1b[23mtest");
    }

    [Fact]
    public void Style_Overline()
    {
        ShouldFormatBe("%C(overline)test%C(^overline)", "\x1b[53mtest\x1b[55m");
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

    [Fact]
    public void SoftReset()
    {
        using var profile = new Profile();
        profile.Rules.Add(new RegexGroupRule(new Regex(@"^z (.*)"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new(FieldRangeList.Parse("1"), "=%Cblue%Gc%Cs="),
        }));
        profile.Rules.Add(new RegexGroupRule(new Regex(@"^y (.*)"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new(FieldRangeList.Parse("1"), "=%Gc%Cs="),
        }));
        profile.Rules.Add(new RegexGroupRule(new Regex(@"^x (.*)"), new List<KeyValuePair<FieldRangeList, string>>
        {
            new(FieldRangeList.Parse("1"), "=%Gc%C(s;bol)="),
        }));
        profile.Build();

        profile.ApplyRules("abc \x1b[31maaa", true).ShouldBe("abc \x1b[31maaa");
        profile.ApplyRules("z test", true).ShouldBe("z =\x1b[34mtest=");
        profile.ApplyRules("z te\x1b[36mst", true).ShouldBe("z =\x1b[34mte\x1b[36mst\x1b[34m=");
        profile.ApplyRules("z\x1b[31m te\x1b[36mst", true).ShouldBe("z\x1b[31m =\x1b[34mte\x1b[36mst\x1b[34m=");

        profile.ApplyRules("abc \x1b[31maaa", true).ShouldBe("abc \x1b[31maaa");
        profile.ApplyRules("y te\x1b[36mst", true).ShouldBe("y =te\x1b[36mst\x1b[31m=");

        profile.ApplyRules("\x1b[0m", true);
        profile.ApplyRules("y test", true).ShouldBe("y =test=");

        profile.ApplyRules("abc \x1b[31maaa", true).ShouldBe("abc \x1b[31maaa");
        profile.ApplyRules("x te\x1b[36mst", true).ShouldBe("x =te\x1b[36mst\x1b[31m\x1b[1m=");
    }

    private static void ShouldFormatBe(string format, string expected)
    {
        ShouldFormatBe(CreateDummyDataState(), format, expected);
    }

    private static void ShouldFormatBe(DataState state, string format, string expected)
    {
        GetFormatter(new ColorFormatBuilder())
            .Format(format, state)
            .ShouldBe(expected);
    }
}
