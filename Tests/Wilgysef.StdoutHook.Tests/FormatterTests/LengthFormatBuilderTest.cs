using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.FormatterTests;

public class LengthFormatBuilderTest : RuleTestBase
{
    [Fact]
    public void Constant()
    {
        var formatter = GetFormatter(new LengthFormatBuilder());

        formatter.Format("%(length:test)", CreateDataState(formatter)).ShouldBe("4");
    }

    [Fact]
    public void NestedFormat()
    {
        using var profile = new Profile();
        profile.Rules.Add(new FieldSeparatorRule(new Regex(@"\s+"))
        {
            ReplaceAllFormat = "%(length:%F1) %F1 %F2",
        });

        profile.Build();

        profile.ApplyRules("test asdf123", true).ShouldBe("4 test asdf123");
    }

    private static DataState CreateDataState(Formatter formatter)
    {
        var profile = new Profile();
        profile.Build(formatter);

        return new DataState(profile);
    }
}
