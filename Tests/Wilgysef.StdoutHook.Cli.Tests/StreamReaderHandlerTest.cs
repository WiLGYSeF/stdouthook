using Shouldly;
using System.Text;

namespace Wilgysef.StdoutHook.Cli.Tests;

public class StreamReaderHandlerTest
{
    [Fact]
    public async Task Newlines()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\ndef\nghi"));
        var expected = new[]
        {
            "abc\n",
            "def\n",
            "ghi",
        };

        await ShouldReadLinesAsync(stream, expected);
    }

    [Fact]
    public async Task CarriageReturnNewlines()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\r\ndef\r\nghi"));
        var expected = new[]
        {
            "abc\r\n",
            "def\r\n",
            "ghi",
        };

        await ShouldReadLinesAsync(stream, expected);
    }

    [Fact]
    public async Task CarriageReturns()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\rdef\rghi"));
        var expected = new[]
        {
            "abc\r",
            "def\r",
            "ghi",
        };

        await ShouldReadLinesAsync(stream, expected);
    }

    [Fact]
    public async Task TrailingNewline()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\ndef\nghi\n"));
        var expected = new[]
        {
            "abc\n",
            "def\n",
            "ghi\n",
        };

        await ShouldReadLinesAsync(stream, expected);
    }

    [Fact]
    public async Task TrailingCarriageReturn()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\ndef\r"));
        var expected = new[]
        {
            "abc\n",
            "def\r",
        };

        await ShouldReadLinesAsync(stream, expected);
    }

    [Fact]
    public async Task DoubleNewline()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\n\ndef\n\n"));
        var expected = new[]
        {
            "abc\n",
            "\n",
            "def\n",
            "\n",
        };

        await ShouldReadLinesAsync(stream, expected);
    }

    [Fact]
    public async Task MultipleRead()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("abcdef\n\n"));
        var expected = new[]
        {
            "abcdef\n",
            "\n",
        };

        await ShouldReadLinesAsync(stream, expected, bufferSize: 4);
    }

    [Fact]
    public async Task MultipleRead_CarriageReturn()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\rdef\n"));
        var expected = new[]
        {
            "abc\r",
            "def\n",
        };

        await ShouldReadLinesAsync(stream, expected, bufferSize: 4);
    }

    [Fact]
    public async Task MultipleRead_CarriageReturn_Newline()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\r\ndef"));
        var expected = new[]
        {
            "abc\r\n",
            "def",
        };

        await ShouldReadLinesAsync(stream, expected, bufferSize: 4);
    }

    [Fact]
    public async Task ForceProcess()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc"));
        using var reader = new TestStreamReader(stream);

        var invocations = 0;
        using var readerHandler = new StreamReaderHandler(reader, data =>
        {
            invocations++;
            data.ShouldBe("abc");
        });

        var task = readerHandler.ReadLinesAsync(forceProcessTimeout: TimeSpan.FromMilliseconds(20));
        await Task.Delay(100);

        invocations.ShouldBe(1);
    }

    private static async Task ShouldReadLinesAsync(
        Stream stream,
        IEnumerable<string> expected,
        int bufferSize = 4096)
    {
        using var reader = new StreamReader(stream);
        using var enumerator = expected.GetEnumerator();

        using var readerHandler = new StreamReaderHandler(reader, data =>
        {
            enumerator.MoveNext().ShouldBeTrue();
            data.ShouldBe(enumerator.Current);
        });

        await readerHandler.ReadLinesAsync(bufferSize: bufferSize);
        enumerator.MoveNext().ShouldBeFalse();
    }

    private class TestStreamReader : StreamReader
    {
        public TestStreamReader(Stream stream) : base(stream) { }

        public override async Task<int> ReadAsync(char[] buffer, int index, int count)
        {
            if (BaseStream.Position == BaseStream.Length)
            {
                // wait forever
                await Task.Delay(-1);
            }

            return await base.ReadAsync(buffer, index, count);
        }
    }
}
