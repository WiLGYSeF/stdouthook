﻿using Wilgysef.StdoutHook.Formatters;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class ColorExtractorTest
{
    [Fact]
    public void Extract()
    {
        var colors = new List<KeyValuePair<int, string>>();
        var data = ColorExtractor.ExtractColor("\x1b[31mtest\x1b[1;46mabc", colors);

        data.ShouldBe("testabc");
        colors.Count.ShouldBe(2);
        colors[0].Key.ShouldBe(0);
        colors[0].Value.ShouldBe("\x1b[31m");
        colors[1].Key.ShouldBe(4);
        colors[1].Value.ShouldBe("\x1b[1;46m");
    }

    [Fact]
    public void Extract_NoColorResults()
    {
        ColorExtractor.ExtractColor("\x1b[31mtest\x1b[1;46mabc").ShouldBe("testabc");
        ColorExtractor.ExtractColor("\x1b[zabc").ShouldBe("\x1b[zabc");
        ColorExtractor.ExtractColor("\x1b[31").ShouldBe("\x1b[31");
    }
}
