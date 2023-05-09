using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class TeeRule : Rule
    {
        public string Filename { get; set; }

        public bool Flush { get; set; }

        public bool ExtractColors { get; set; }

        private string _absolutePath;

        internal override void Build(ProfileState state, Formatter formatter)
        {
            base.Build(state, formatter);

            _absolutePath = Path.GetFullPath(Filename);
        }

        internal override string Apply(DataState state)
        {
            FileStream? factoryStream = null;
            var lockedStream = state.ProfileState.FileStreams.GetOrAdd(_absolutePath, CreateStream);

            if (factoryStream != null && factoryStream != lockedStream?.Stream)
            {
                // a stream was created but the key already exists
                factoryStream.Dispose();
                factoryStream = null;
            }

            if (lockedStream == null)
            {
                return state.Data;
            }

            var data = Encoding.UTF8.GetBytes(ExtractColors
                ? ColorExtractor.ExtractColor(state.Data, new List<KeyValuePair<int, string>>())
                : state.Data);

            lock (lockedStream.Lock)
            {
                lockedStream.Stream.Write(data);

                if (Flush)
                {
                    lockedStream.Stream.Flush();
                }
            }

            return state.Data;

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
