using System;
using System.IO;
using System.Text;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class TeeRule : Rule
    {
        public string Filename
        {
            get => _filename;
            set
            {
                _filename = value;
                _absolutePath = Path.GetFullPath(_filename);
            }
        }

        public bool Flush { get; set; }

        private string _filename;
        private string _absolutePath;

        public override string Apply(string data, bool stdout, ProfileState state)
        {
            FileStream? factoryStream = null;
            var lockedStream = state.FileStreams.GetOrAdd(_absolutePath, CreateStream);

            if (factoryStream != null && factoryStream != lockedStream?.Stream)
            {
                // a stream was created but the key already exists
                factoryStream.Dispose();
                factoryStream = null;
            }

            if (lockedStream == null)
            {
                return data;
            }

            lock (lockedStream.Lock)
            {
                lockedStream.Stream.Write(Encoding.UTF8.GetBytes(data));

                if (Flush)
                {
                    lockedStream.Stream.Flush();
                }
            }

            return data;

            ProfileState.LockedFileStream? CreateStream(string key)
            {
                try
                {
                    factoryStream = new FileStream(key, FileMode.Append);
                    return new ProfileState.LockedFileStream(factoryStream);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}
