using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class ProfileFormatBuilderTest : RuleTestBase
{
    [Fact]
    public void ProfileName()
    {
        var formatter = GetFormatter(new ProfileFormatBuilder());

        using var profile = new Profile
        {
            ProfileName = "test",
        };

        formatter.Format("%(profile:name)", new DataState(profile)).ShouldBe("test");
    }

    [Fact]
    public void StdoutLines()
    {
        var formatter = GetFormatter(new ProfileFormatBuilder());

        using var profile = new Profile();
        profile.State.StdoutLineCount = 123;

        formatter.Format("%(profile:stdoutLines)", new DataState(profile)).ShouldBe("123");
    }

    [Fact]
    public void StderrLines()
    {
        var formatter = GetFormatter(new ProfileFormatBuilder());

        using var profile = new Profile();
        profile.State.StderrLineCount = 123;

        formatter.Format("%(profile:stderrLines)", new DataState(profile)).ShouldBe("123");
    }

    [Fact]
    public void TotalLines()
    {
        var formatter = GetFormatter(new ProfileFormatBuilder());

        using var profile = new Profile();
        profile.State.StdoutLineCount = 100;
        profile.State.StderrLineCount = 23;

        formatter.Format("%(profile:totalLines)", new DataState(profile)).ShouldBe("123");
    }

    [Fact]
    public void Invalid()
    {
        var formatter = GetFormatter(new ProfileFormatBuilder());

        using var profile = new Profile();
        formatter.Format("%(profile:notexist)", new DataState(profile)).ShouldBe("%(profile:notexist)");
    }
}
