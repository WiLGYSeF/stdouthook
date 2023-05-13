using System;
using System.IO;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class ConcurrentStream : IDisposable
    {
        public bool Flush { get; set; }

        private readonly Stream _stream;

        private readonly object _lock = new object();

        public ConcurrentStream(Stream stream, bool flush = false)
        {
            Flush = flush;
            _stream = stream;
        }

        public void Write(byte[] data, bool flush = false)
        {
            lock (_lock)
            {
                _stream.Write(data);

                if (flush || Flush)
                {
                    _stream.Flush();
                }
            }
        }

        public bool IsStream(Stream stream)
        {
            return stream == _stream;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
