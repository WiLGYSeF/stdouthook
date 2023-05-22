using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Wilgysef.StdoutHook.Loggers;

namespace Wilgysef.StdoutHook.Profiles
{
    public class ProfileState : IDisposable
    {
        public Process Process { get; private set; } = null!;

        public long StdoutLineCount { get; set; }

        public long StderrLineCount { get; set; }

        public long LineCount => StdoutLineCount + StderrLineCount;

        internal Func<string, Stream> StreamFactory { get; set; } =
            absolutePath => new FileStream(absolutePath, FileMode.Append, FileAccess.Write, FileShare.Read);

        private readonly ConcurrentDictionary<string, ConcurrentStream> _fileStreams = new ConcurrentDictionary<string, ConcurrentStream>();

        public void SetProcess(Process process)
        {
            Process = process;
        }

        public void Dispose()
        {
            Process?.Dispose();

            foreach (var lockedStream in _fileStreams.Values)
            {
                lockedStream?.Dispose();
            }

            _fileStreams.Clear();
        }

        internal ConcurrentStream GetOrCreateFileStream(string absolutePath)
        {
            ConcurrentStream stream;
            Stream? createdStream = null;

            try
            {
                stream = _fileStreams.GetOrAdd(absolutePath, CreateStream);

                if (createdStream != null && !stream.IsStream(createdStream))
                {
                    // a stream was created but the key already exists
                    createdStream.Dispose();
                    createdStream = null;
                }
            }
            catch (Exception ex)
            {
                if (!_fileStreams.TryGetValue(absolutePath, out stream))
                {
                    // wait a small amount in case the stream was created after another stream was created but before it was added to the dictionary
                    Thread.Sleep(50);

                    if (!_fileStreams.TryGetValue(absolutePath, out stream))
                    {
                        throw;
                    }
                }

                // exception was thrown but the stream exists, continue
                // GlobalLogger.Warn($"an exception was thrown when opening the file stream, although the file stream exists: {ex.Message}: {absolutePath}");
            }

            return stream;

            ConcurrentStream CreateStream(string key)
            {
                createdStream = StreamFactory(key);
                return new ConcurrentStream(createdStream);
            }
        }
    }
}
