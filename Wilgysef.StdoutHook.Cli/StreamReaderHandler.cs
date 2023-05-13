using System.Text;

namespace Wilgysef.StdoutHook.Cli;

public class StreamReaderHandler
{
    private readonly StreamReader _reader;
    private readonly Action<string> _action;

    public StreamReaderHandler(StreamReader reader, Action<string> action)
    {
        _reader = reader;
        _action = action;
    }

    public async Task ReadLinesAsync(int bufferSize = 4096, CancellationToken cancellationToken = default)
    {
        await Task.Yield();

        var builder = new StringBuilder();
        var buffer = new char[bufferSize];
        var endedWithCr = false;
        int bytesRead;

        do
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: cancellation token?
            bytesRead = await _reader.ReadAsync(buffer, 0, buffer.Length);
            var index = 0;

            if (endedWithCr)
            {
                if (bytesRead > 0 && buffer[0] == '\n')
                {
                    builder.Append('\n');
                    index = 1;
                }

                _action(builder.ToString());
                builder.Clear();
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
                            data = builder.Append(new Span<char>(buffer, last, index - last + 2))
                                .ToString();
                            last = index + 2;
                            index++;
                        }
                        else
                        {
                            data = builder.Append(new Span<char>(buffer, last, index - last + 1))
                                .ToString();
                            last = index + 1;
                        }

                        _action(data);
                        builder.Clear();
                    }
                    else
                    {
                        endedWithCr = true;
                    }
                }
                else if (buffer[index] == '\n')
                {
                    _action(builder.Append(new Span<char>(buffer, last, index - last + 1)).ToString());
                    builder.Clear();
                    last = index + 1;
                }
            }

            if (bytesRead - last > 0)
            {
                builder.Append(new Span<char>(buffer, last, bytesRead - last));
            }
        }
        while (bytesRead != 0);

        if (builder.Length > 0)
        {
            _action(builder.ToString());
        }
    }
}
