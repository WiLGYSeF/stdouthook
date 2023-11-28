using System.Text;

namespace Wilgysef.StdoutHook.Cli;

public class CustomStreamWriter : TextWriter
{
    private readonly Stream _stream;
    private readonly Encoding _encoding;
    private readonly char[] _buffer;
    private readonly byte[] _byteBuffer;
    private readonly object _lock = new();

    private int _bufferIndex;

    public CustomStreamWriter(Stream stream, Encoding encoding, int bufferSize)
    {
        _stream = stream;
        _encoding = encoding;
        _buffer = new char[bufferSize];
        _byteBuffer = new byte[_encoding.GetMaxByteCount(bufferSize)];
    }

    public override Encoding Encoding => _encoding;

    // need to override the TextWriter Write(char)
    public override void Write(char value)
    {
        lock (_lock)
        {
            if (_bufferIndex == _buffer.Length)
            {
                Flush();
            }

            _buffer[_bufferIndex++] = value;
        }
    }

    public override void Write(string? value)
    {
        if (value == null)
        {
            return;
        }

        lock (_lock)
        {
            var forcedFlush = false;

            if (_bufferIndex + value.Length >= _buffer.Length)
            {
                forcedFlush = FlushInternal();
            }

            if (forcedFlush || value.Length >= _buffer.Length)
            {
                _stream.Write(_encoding.GetBytes(value));
            }
            else
            {
                value.CopyTo(0, _buffer, _bufferIndex, value.Length);
                _bufferIndex += value.Length;
            }
        }
    }

    public override void Flush()
    {
        FlushInternal();
    }

    protected override void Dispose(bool disposing)
    {
        Flush();
    }

    private bool FlushInternal()
    {
        lock (_lock)
        {
            var forced = false;

            int endIndex;
            for (endIndex = _bufferIndex - 1; endIndex >= 0; endIndex--)
            {
                if (_buffer[endIndex] == '\r' || _buffer[endIndex] == '\n')
                {
                    endIndex++;
                    break;
                }
            }

            if (endIndex <= 0)
            {
                // no choice but to flush entire buffer
                endIndex = _bufferIndex;
                forced = true;
            }

            var bytesWritten = _encoding.GetBytes(_buffer, 0, endIndex, _byteBuffer, 0);
            _stream.Write(_byteBuffer, 0, bytesWritten);
            _stream.Flush();

            Array.Copy(_buffer, endIndex, _buffer, 0, _bufferIndex - endIndex);
            _bufferIndex -= endIndex;

            return forced;
        }
    }
}
