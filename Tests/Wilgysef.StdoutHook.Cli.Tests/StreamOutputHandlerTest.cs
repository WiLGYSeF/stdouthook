using Shouldly;
using System.Text;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Cli.Tests;

public class StreamOutputHandlerTest
{
    [Fact]
    public async Task HandleData()
    {
        var stdout = new MemoryStream();
        var stderr = new MemoryStream();

        var profile = CreateTestProfile();
        profile.Build();

        await ProcessOutputHandlerAsync(
            profile,
            CreateStream("abc\ndef\n"),
            CreateStream("123\n"),
            stdout,
            stderr);

        Encoding.UTF8.GetString(stdout.ToArray()).ShouldBe("test abc\ntest def\n");
        Encoding.UTF8.GetString(stderr.ToArray()).ShouldBe("test 123\n");

        profile.State.StdoutLineCount.ShouldBe(2);
        profile.State.StderrLineCount.ShouldBe(1);
    }

    [Fact]
    public async Task HandleData_Flush()
    {
        var stdout = new MemoryStream();
        var stderr = new MemoryStream();

        var profile = CreateTestProfile();
        profile.Flush = true;
        profile.Build();

        await ProcessOutputHandlerAsync(
            profile,
            CreateStream("abc\n"),
            CreateStream("def\n"),
            stdout,
            stderr);

        Encoding.UTF8.GetString(stdout.ToArray()).ShouldBe("test abc\n");
        Encoding.UTF8.GetString(stderr.ToArray()).ShouldBe("test def\n");

        profile.State.StdoutLineCount.ShouldBe(1);
        profile.State.StderrLineCount.ShouldBe(1);
    }

    private static async Task ProcessOutputHandlerAsync(
        Profile profile,
        Stream stdoutInput,
        Stream stderrInput,
        Stream stdoutOutput,
        Stream stderrOutput)
    {
        var stdoutWriter = new StreamWriter(stdoutOutput);
        var stderrWriter = new StreamWriter(stderrOutput);

        var handler = new StreamOutputHandler(
            profile,
            new StreamReader(stdoutInput),
            new StreamReader(stderrInput),
            stdoutWriter,
            stderrWriter);

        await handler.ReadLinesAsync();

        stdoutWriter.Flush();
        stderrWriter.Flush();
    }

    private static Stream CreateStream(string data)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(data));
    }

    private static Profile CreateTestProfile()
    {
        return new Profile
        {
            Rules = new List<Rule>
            {
                new UnconditionalReplaceRule("test %data"),
            }
        };
    }
}
