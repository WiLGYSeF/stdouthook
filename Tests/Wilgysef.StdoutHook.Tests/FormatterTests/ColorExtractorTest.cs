using Wilgysef.StdoutHook.Formatters;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class ColorExtractorTest
{
    [Fact]
    public void Extract()
    {
        var colors = new List<KeyValuePair<int, string>>();
        var data = ColorExtractor.ExtractColor("\x1b[31mtest\x1b[1;46mab\x1b[32mc", colors);

        data.ShouldBe("testabc");
        colors.Count.ShouldBe(3);
        colors[0].Key.ShouldBe(0);
        colors[0].Value.ShouldBe("\x1b[31m");
        colors[1].Key.ShouldBe(4);
        colors[1].Value.ShouldBe("\x1b[1;46m");
        colors[2].Key.ShouldBe(6);
        colors[2].Value.ShouldBe("\x1b[32m");
    }

    [Fact]
    public void Extract_NoColorResults()
    {
        ColorExtractor.ExtractColor("\x1b[31mtest\x1b[1;46mabc", null).ShouldBe("testabc");
        ColorExtractor.ExtractColor("\x1b[zabc", null).ShouldBe("\x1b[zabc");
        ColorExtractor.ExtractColor("\x1b[31", null).ShouldBe("\x1b[31");
    }
}
