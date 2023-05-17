using System.Text;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class TeeRuleTest : RuleTestBase
{
    [Fact]
    public void Write()
    {
        var rule = new TeeRule("testfile");
        ShouldRuleBe(rule, new[] { "test\n" }, "test\n");
    }

    [Fact]
    public void ExtractColors()
    {
        var rule = new TeeRule("testfile");
        ShouldRuleBe(rule, new[] { "\x1b[31mtest\n" }, "\x1b[31mtest\n");

        rule = new TeeRule("testfile")
        {
            ExtractColors = true,
        };
        ShouldRuleBe(rule, new[] { "\x1b[31mtest\n" }, "test\n");
    }

    [Fact]
    public void Flush()
    {
        var rule = new TeeRule("testfile")
        {
            Flush = true,
        };
        ShouldRuleBe(rule, new[] { "test\n" }, "test\n");
    }

    private static void ShouldRuleBe(TeeRule rule, IEnumerable<string> lines, string expected)
    {
        using var stream = new MemoryStream();

        using var profile = new Profile();
        profile.State.StreamFactory = absolutePath =>
        {
            absolutePath.ShouldBe(Path.Combine(Environment.CurrentDirectory, rule.Filename));
            return stream;
        };

        profile.Rules.Add(rule);
        profile.Build();

        foreach (var line in lines)
        {
            rule.Apply(new DataState(line, true, profile));
        }

        Encoding.UTF8.GetString(stream.ToArray()).ShouldBe(expected);
    }
}
