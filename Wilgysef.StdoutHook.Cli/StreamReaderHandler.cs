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

    public async Task ReadLinesAsync(int bufferSize = 2048, CancellationToken cancellationToken = default)
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
                        if (buffer[index + 1] == '\n')
                        {
                            builder.Append(new Span<char>(buffer, last, index - last + 2));
                            last = index + 2;
                            index++;
                        }
                        else
                        {
                            builder.Append(new Span<char>(buffer, last, index - last + 1));
                            last = index + 1;
                        }

                        _action(builder.ToString());
                        builder.Clear();
                    }
                    else
                    {
                        endedWithCr = true;
                    }
                }
                else if (buffer[index] == '\n')
                {
                    builder.Append(new Span<char>(buffer, last, index - last + 1));
                    _action(builder.ToString());
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
