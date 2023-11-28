using Wilgysef.StdoutHook.Formatters.FormatBuilders;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class ByteFormatBuilderTest : RuleTestBase
{
    [Fact]
    public void Byte()
    {
        var formatter = GetFormatter(new ByteFormatBuilder());

        formatter.Format("test%x41", CreateDummyDataState()).ShouldBe("testA");
    }

    [Fact]
    public void Invalid()
    {
        var formatter = GetFormatter(new ByteFormatBuilder());

        formatter.Format("test%xgh", CreateDummyDataState()).ShouldBe("test");
    }
}
