using Wilgysef.StdoutHook.Formatters.FormatBuilders;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class TimeFormatBuilderTest : RuleTestBase
{
    [Fact]
    public void Time()
    {
        var formatter = GetFormatter(new TimeFormatBuilder());

        var now = DateTime.Now;
        var result = formatter.Format("%(time:u)", CreateDummyDataState("test\n", true));
        var resultDate = DateTime.Parse(result);

        (resultDate - now).TotalSeconds.ShouldBeLessThan(2);
    }
}
