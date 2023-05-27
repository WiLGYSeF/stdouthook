using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class ColorExtractorTest
{
    [Fact]
    public void Extract()
    {
        var colors = new ColorList();
        var data = ColorExtractor.ExtractColor("\x1b[31mtest\x1b[1;46mab\x1b[32mc", colors);

        data.ShouldBe("testabc");
        colors.Count.ShouldBe(3);
        colors[0].Position.ShouldBe(0);
        colors[0].Color.ToString().ShouldBe("\x1b[31m");
        colors[1].Position.ShouldBe(4);
        colors[1].Color.ToString().ShouldBe("\x1b[1;46m");
        colors[2].Position.ShouldBe(6);
        colors[2].Color.ToString().ShouldBe("\x1b[32m");
    }

    [Fact]
    public void Extract_Consecutive()
    {
        var colors = new ColorList();
        var data = ColorExtractor.ExtractColor("\x1b[31mtest\x1b[0m\x1b[31ma", colors);

        data.ShouldBe("testa");
        colors.Count.ShouldBe(3);
        colors[0].Position.ShouldBe(0);
        colors[0].Color.ToString().ShouldBe("\x1b[31m");
        colors[1].Position.ShouldBe(4);
        colors[1].Color.ToString().ShouldBe("\x1b[0m");
        colors[2].Position.ShouldBe(4);
        colors[2].Color.ToString().ShouldBe("\x1b[31m");
    }

    [Fact]
    public void Extract_End()
    {
        var colors = new ColorList();
        var data = ColorExtractor.ExtractColor("\x1b[31mtest\x1b[0m", colors);

        data.ShouldBe("test");
        colors.Count.ShouldBe(2);
        colors[0].Position.ShouldBe(0);
        colors[0].Color.ToString().ShouldBe("\x1b[31m");
        colors[1].Position.ShouldBe(4);
        colors[1].Color.ToString().ShouldBe("\x1b[0m");
    }

    [Fact]
    public void Extract_NoColorResults()
    {
        ColorExtractor.ExtractColor("\x1b[31mtest\x1b[1;46mabc", null).ShouldBe("testabc");
        ColorExtractor.ExtractColor("\x1b[zabc", null).ShouldBe("\x1b[zabc");
        ColorExtractor.ExtractColor("\x1b[31", null).ShouldBe("\x1b[31");
    }
}
