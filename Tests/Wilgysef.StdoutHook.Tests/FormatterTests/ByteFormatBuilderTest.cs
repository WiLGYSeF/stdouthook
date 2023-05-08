using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class ByteFormatBuilderTest : RuleTestBase
{
    [Fact]
    public void Byte()
    {
        var formatter = GetFormatter(new ByteFormatBuilder());

        formatter.Format("test%x41", new DataState(new ProfileState())).ShouldBe("testA");
    }

    [Fact]
    public void Invalid()
    {
        var formatter = GetFormatter(new ByteFormatBuilder());

        formatter.Format("test%xgh", new DataState(new ProfileState())).ShouldBe("test");
    }
}
