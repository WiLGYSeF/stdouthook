using Wilgysef.StdoutHook.Formatters.FormatBuilders;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class DataFormatBuilderTest : RuleTestBase
{
    [Fact]
    public void Data()
    {
        var formatter = GetFormatter(new DataFormatBuilder());

        formatter.Format("%data", CreateDummyDataState("test\n", true)).ShouldBe("test");
    }

    [Fact]
    public void NoTrim()
    {
        var formatter = GetFormatter(new DataFormatBuilder());

        formatter.Format("%(data:notrim)", CreateDummyDataState("test\n", true)).ShouldBe("test\n");
    }

    [Fact]
    public void Newline()
    {
        var formatter = GetFormatter(new DataFormatBuilder());

        formatter.Format("%(data:newline)", CreateDummyDataState("test\n", true)).ShouldBe("\n");
        formatter.Format("%(data:newline)", CreateDummyDataState("test\r\n", true)).ShouldBe("\r\n");
        formatter.Format("%(data:newline)", CreateDummyDataState("test\r", true)).ShouldBe("\r");
    }
}
