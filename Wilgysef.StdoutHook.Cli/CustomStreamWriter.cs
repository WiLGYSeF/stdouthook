using System.Text;

namespace Wilgysef.StdoutHook.Cli;

public class CustomStreamWriter : TextWriter
{
    public override Encoding Encoding => _encoding;

    private readonly Stream _stream;
    private readonly Encoding _encoding;
    private readonly char[] _buffer;
    private readonly byte[] _byteBuffer;

    private int _bufferIndex;
    private object _lock = new();

    public CustomStreamWriter(Stream stream, Encoding encoding, int bufferSize)
    {
        _stream = stream;
        _encoding = encoding;
        _buffer = new char[bufferSize];
        _byteBuffer = new byte[_encoding.GetMaxByteCount(bufferSize)];
    }

    public void SetSharedLock(object lockObject)
    {
        _lock = lockObject;
    }

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
            if (_bufferIndex + value.Length >= _buffer.Length)
            {
                Flush();
            }

            if (value.Length >= _buffer.Length)
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
        lock (_lock)
        {
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
            }

            var bytesWritten = _encoding.GetBytes(_buffer, 0, endIndex, _byteBuffer, 0);
            _stream.Write(_byteBuffer, 0, bytesWritten);
            _stream.Flush();

            Array.Copy(_buffer, endIndex, _buffer, 0, _bufferIndex - endIndex);
            _bufferIndex -= endIndex;
        }
    }

    protected override void Dispose(bool disposing)
    {
        Flush();
    }
}
