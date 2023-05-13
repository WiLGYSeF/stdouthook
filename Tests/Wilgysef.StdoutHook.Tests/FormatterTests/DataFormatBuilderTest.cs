using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class DataFormatBuilderTest : RuleTestBase
{
    [Fact]
    public void Data()
    {
        var formatter = GetFormatter(new DataFormatBuilder());

        formatter.Format("%data", new DataState("test\n", true, new ProfileState())).ShouldBe("test");
    }

    [Fact]
    public void NoTrim()
    {
        var formatter = GetFormatter(new DataFormatBuilder());

        formatter.Format("%(data:notrim)", new DataState("test\n", true, new ProfileState())).ShouldBe("test\n");
    }
}
