using Shouldly;
using System.Text;

namespace Wilgysef.StdoutHook.Cli.Tests;

public class CustomStreamWriterTest
{
    [Fact]
    public void Write_Char()
    {
        using var stream = new MemoryStream();
        using var streamWriter = new CustomStreamWriter(stream, Encoding.UTF8, 4);

        streamWriter.Write('a');
        stream.ToArray().Length.ShouldBe(0);

        streamWriter.Write('b');
        stream.ToArray().Length.ShouldBe(0);

        streamWriter.Write('\n');
        stream.ToArray().Length.ShouldBe(0);

        streamWriter.Write('c');
        stream.ToArray().Length.ShouldBe(0);

        streamWriter.Write('d');
        Encoding.UTF8.GetString(stream.ToArray()).ShouldBe("ab\n");
    }

    [Fact]
    public void Write_Char_TooLong()
    {
        using var stream = new MemoryStream();
        using var streamWriter = new CustomStreamWriter(stream, Encoding.UTF8, 4);

        streamWriter.Write('a');
        stream.ToArray().Length.ShouldBe(0);

        streamWriter.Write('b');
        stream.ToArray().Length.ShouldBe(0);

        streamWriter.Write('c');
        stream.ToArray().Length.ShouldBe(0);

        streamWriter.Write('d');
        stream.ToArray().Length.ShouldBe(0);

        streamWriter.Write('e');
        Encoding.UTF8.GetString(stream.ToArray()).ShouldBe("abcd");
    }

    [Fact]
    public void Write_String_Null()
    {
        using var stream = new MemoryStream();
        using var streamWriter = new CustomStreamWriter(stream, Encoding.UTF8, 6);
        streamWriter.Encoding.ShouldBe(Encoding.UTF8);

        streamWriter.Write((string?)null);

        stream.ToArray().Length.ShouldBe(0);
    }

    [Fact]
    public void Write_String_Buffered()
    {
        using var stream = new MemoryStream();
        using var streamWriter = new CustomStreamWriter(stream, Encoding.UTF8, 6);

        streamWriter.Write("test\n");
        stream.ToArray().Length.ShouldBe(0);

        streamWriter.Write("asdf\n");
        Encoding.UTF8.GetString(stream.ToArray()).ShouldBe("test\n");

        streamWriter.Write("abcd\n");
        Encoding.UTF8.GetString(stream.ToArray()).ShouldBe("test\nasdf\n");
    }

    [Fact]
    public void Write_String_Buffered_ForcedFlush()
    {
        using var stream = new MemoryStream();
        using var streamWriter = new CustomStreamWriter(stream, Encoding.UTF8, 6);

        streamWriter.Write("test");
        stream.ToArray().Length.ShouldBe(0);

        streamWriter.Write("asdf\n");
        Encoding.UTF8.GetString(stream.ToArray()).ShouldBe("testasdf\n");
    }

    [Fact]
    public void Write_String_TooLong()
    {
        using var stream = new MemoryStream();
        using var streamWriter = new CustomStreamWriter(stream, Encoding.UTF8, 6);

        streamWriter.Write("test\n");
        stream.ToArray().Length.ShouldBe(0);

        streamWriter.Write("asdfasdf\n");
        Encoding.UTF8.GetString(stream.ToArray()).ShouldBe("test\nasdfasdf\n");

        streamWriter.Write("asdf\n");
        Encoding.UTF8.GetString(stream.ToArray()).ShouldBe("test\nasdfasdf\n");

        streamWriter.Write("abcd\n");
        Encoding.UTF8.GetString(stream.ToArray()).ShouldBe("test\nasdfasdf\nasdf\n");
    }
}
