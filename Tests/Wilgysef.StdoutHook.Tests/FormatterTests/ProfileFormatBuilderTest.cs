using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class ProfileFormatBuilderTest : RuleTestBase
{
    [Fact]
    public void ProfileName()
    {
        var formatter = GetFormatter(new ProfileFormatBuilder());

        var profile = new Profile
        {
            ProfileName = "test",
        };

        formatter.Format("%(profile:name)", new DataState(profile)).ShouldBe("test");
    }

    [Fact]
    public void StdoutLines()
    {
        var formatter = GetFormatter(new ProfileFormatBuilder());

        var state = new ProfileState()
        {
            StdoutLineCount = 123,
        };
        var profile = new Profile(state);

        formatter.Format("%(profile:stdoutLines)", new DataState(profile)).ShouldBe("123");
    }

    [Fact]
    public void StderrLines()
    {
        var formatter = GetFormatter(new ProfileFormatBuilder());

        var state = new ProfileState()
        {
            StderrLineCount = 123,
        };
        var profile = new Profile(state);

        formatter.Format("%(profile:stderrLines)", new DataState(profile)).ShouldBe("123");
    }

    [Fact]
    public void TotalLines()
    {
        var formatter = GetFormatter(new ProfileFormatBuilder());

        var state = new ProfileState()
        {
            StdoutLineCount = 100,
            StderrLineCount = 23,
        };
        var profile = new Profile(state);

        formatter.Format("%(profile:totalLines)", new DataState(profile)).ShouldBe("123");
    }
}
