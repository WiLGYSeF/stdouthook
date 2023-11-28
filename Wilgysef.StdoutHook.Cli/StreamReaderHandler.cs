using System.Text;

namespace Wilgysef.StdoutHook.Cli;

public class StreamReaderHandler : IDisposable
{
    private readonly StreamReader _reader;
    private readonly Action<string> _action;

    private readonly StringBuilder _builder = new();

    public StreamReaderHandler(StreamReader reader, Action<string> action)
    {
        _reader = reader;
        _action = action;
    }

    public async Task ReadLinesAsync(
        int bufferSize = 4096,
        TimeSpan? forceProcessTimeout = null,
        CancellationToken cancellationToken = default)
    {
        await Task.Yield();

        var buffer = new char[bufferSize];
        var endedWithCr = false;
        int bytesRead;
        int? waitMs = forceProcessTimeout.HasValue
            ? (int)forceProcessTimeout.Value.TotalMilliseconds
            : null;

        _builder.Clear();

        do
        {
            cancellationToken.ThrowIfCancellationRequested();

            var readTask = _reader.ReadAsync(buffer, 0, buffer.Length);
            if (waitMs.HasValue
                && !readTask.Wait(waitMs.Value, cancellationToken)
                && _builder.Length > 0)
            {
                _action(_builder.ToString());
                _builder.Clear();
            }

            bytesRead = await readTask;
            var index = 0;

            if (endedWithCr)
            {
                if (bytesRead > 0 && buffer[0] == '\n')
                {
                    _builder.Append('\n');
                    index = 1;
                }

                _action(_builder.ToString());
                _builder.Clear();

                endedWithCr = false;
            }

            var last = index;
            for (; index < bytesRead; index++)
            {
                if (buffer[index] == '\r')
                {
                    if (index != bytesRead - 1)
                    {
                        string data;
                        if (buffer[index + 1] == '\n')
                        {
                            data = _builder.Append(new Span<char>(buffer, last, index - last + 2))
                                .ToString();
                            last = index + 2;
                            index++;
                        }
                        else
                        {
                            data = _builder.Append(new Span<char>(buffer, last, index - last + 1))
                                .ToString();
                            last = index + 1;
                        }

                        _action(data);
                        _builder.Clear();
                    }
                    else
                    {
                        endedWithCr = true;
                    }
                }
                else if (buffer[index] == '\n')
                {
                    _action(_builder.Append(new Span<char>(buffer, last, index - last + 1)).ToString());
                    _builder.Clear();
                    last = index + 1;
                }
            }

            if (bytesRead - last > 0)
            {
                _builder.Append(new Span<char>(buffer, last, bytesRead - last));
            }
        }
        while (bytesRead != 0);

        if (_builder.Length > 0)
        {
            _action(_builder.ToString());
            _builder.Clear();
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (_builder.Length > 0)
        {
            _action(_builder.ToString());
        }
    }
}
