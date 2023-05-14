using Wilgysef.StdoutHook.Formatters;

namespace Wilgysef.StdoutHook.Tests.FormatterTests
{
    public class ColorFormatterTest
    {
        [Fact]
        public void Format()
        {
            new ColorFormatter().Format("%C(red)test").ShouldBe("\x1b[31mtest");
        }

        [Fact]
        public void CustomColors()
        {
            var formatter = new ColorFormatter();
            formatter.CustomColors.Add("test", "red");

            formatter.Format("%C(test)abc").ShouldBe("\x1b[31mabc");
        }
    }
}
