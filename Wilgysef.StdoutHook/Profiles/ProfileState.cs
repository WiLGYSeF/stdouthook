using System;
using System.Collections.Concurrent;
using System.IO;

namespace Wilgysef.StdoutHook.Profiles
{
    public class ProfileState : IDisposable
    {
        public long StdoutLineCount { get; set; }

        public long StderrLineCount { get; set; }

        public long LineCount => StdoutLineCount + StderrLineCount;

        public ConcurrentDictionary<string, LockedFileStream?> FileStreams { get; } = new ConcurrentDictionary<string, LockedFileStream?>();

        public void Dispose()
        {
            foreach (var stream in FileStreams.Values)
            {
                stream?.Dispose();
            }

            FileStreams.Clear();
        }

        public class LockedFileStream : IDisposable
        {
            public FileStream Stream { get; }

            public object Lock = new object();

            public LockedFileStream(FileStream stream)
            {
                Stream = stream;
            }

            public void Dispose()
            {
                Stream?.Dispose();
            }
        }
    }
}
