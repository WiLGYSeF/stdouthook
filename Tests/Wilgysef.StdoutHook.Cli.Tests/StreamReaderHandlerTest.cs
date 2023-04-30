using Shouldly;
using System.Text;

namespace Wilgysef.StdoutHook.Cli.Tests;

public class StreamReaderHandlerTest
{
    [Fact]
    public async Task Newlines()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\ndef\nghi"));
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
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\r\ndef\r\nghi"));
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
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\rdef\rghi"));
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
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\ndef\nghi\n"));
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
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\ndef\r"));
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
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\n\ndef\n\n"));
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
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("abcdef\n\n"));
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
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\rdef\n"));
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
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("abc\r\ndef"));
        var expected = new[]
        {
            "abc\r\n",
            "def",
        };

        await ShouldReadLinesAsync(stream, expected, bufferSize: 4);
    }

    private static async Task ShouldReadLinesAsync(
        Stream stream,
        IEnumerable<string> expected,
        int bufferSize = 2048)
    {
        using var reader = new StreamReader(stream);
        using var enumerator = expected.GetEnumerator();

        var readerHandler = new StreamReaderHandler(reader, data =>
        {
            enumerator.MoveNext().ShouldBeTrue();
            data.ShouldBe(enumerator.Current);
        });

        await readerHandler.ReadLinesAsync(bufferSize: bufferSize);
        enumerator.MoveNext().ShouldBeFalse();
    }
}
