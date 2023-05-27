using System;
using System.IO;
using System.Text;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Loggers;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class TeeRule : Rule
    {
        public string Filename { get; set; }

        public bool Flush { get; set; }

        public bool ExtractColors { get; set; }

        private string _absolutePath = null!;
        private ConcurrentStream? _stream;

        public TeeRule(string filename)
        {
            Filename = filename;
        }

        internal override void Build(Profile profile, Formatter formatter)
        {
            base.Build(profile, formatter);

            _absolutePath = Path.GetFullPath(Filename);
        }

        internal override string Apply(DataState state)
        {
            // profile state remains the same
            _stream ??= state.Profile.State.GetOrCreateFileStream(_absolutePath);

            var data = Encoding.UTF8.GetBytes(ExtractColors
                ? state.DataExtractedColorTrimEndNewline + state.Newline
                : state.Data);

            try
            {
                _stream.Write(data, Flush);
            }
            catch (Exception ex)
            {
                GlobalLogger.Error($"failed to write output to file: {ex.Message}: {_absolutePath}");
            }

            return state.Data;
        }

        protected override Rule CopyInternal()
        {
            return new TeeRule(Filename)
            {
                Flush = Flush,
                ExtractColors = ExtractColors,
                _absolutePath = _absolutePath,
            };
        }
    }
}
