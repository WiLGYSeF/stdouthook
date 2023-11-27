using System;
using System.IO;

namespace Wilgysef.StdoutHook.Profiles;

/// <summary>
/// Wrapper for concurrent writing to a stream.
/// </summary>
internal class ConcurrentStream : IDisposable
{
    private readonly Stream _stream;

    private readonly object _lock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentStream"/> class.
    /// </summary>
    /// <param name="stream">Stream.</param>
    /// <param name="flush">Whether to flush the stream after writing.</param>
    public ConcurrentStream(Stream stream, bool flush = false)
    {
        AutoFlush = flush;
        _stream = stream;
    }

    /// <summary>
    /// Whether the stream flushes after writing.
    /// </summary>
    public bool AutoFlush { get; set; }

    /// <summary>
    /// Writes to the stream.
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="flush">Whether to flush after writing.</param>
    public void Write(byte[] data, bool flush = false)
    {
        lock (_lock)
        {
            _stream.Write(data);

            if (flush || AutoFlush)
            {
                _stream.Flush();
            }
        }
    }

    /// <summary>
    /// Flushes the stream.
    /// </summary>
    public void Flush()
    {
        lock (_lock)
        {
            _stream.Flush();
        }
    }

    /// <summary>
    /// Checks if <paramref name="stream"/> is the wrapped stream.
    /// </summary>
    /// <param name="stream">Stream to compare.</param>
    /// <returns><see langword="true"/> if the stream matches, otherwise <see langword="false"/>.</returns>
    public bool IsStream(Stream stream)
    {
        return stream == _stream;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _stream.Dispose();
    }
}
