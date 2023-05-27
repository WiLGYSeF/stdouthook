using Wilgysef.StdoutHook.Extensions;
using Wilgysef.StdoutHook.Formatters;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class DataState
    {
        public string Data
        {
            get => _data;
            internal set
            {
                if (_data != value)
                {
                    _data = value!;
                    DataTrimEndNewline = _data.TrimEndNewline(out _newline);

                    _extractedColors.Clear();
                    DataExtractedColorTrimEndNewline = ColorExtractor.ExtractColor(DataTrimEndNewline, _extractedColors);
                }
            }
        }

        public string DataTrimEndNewline { get; private set; } = null!;

        public string DataExtractedColorTrimEndNewline { get; private set; } = null!;

        public string Newline => _newline;

        public ColorList ExtractedColors => _extractedColors;

        public bool Stdout { get; }

        public Profile Profile { get; }

        public RuleContext Context { get; } = new();

        internal int LastColorFormatIndex { get; set; } = -1;

        private readonly ColorList _extractedColors = new();
        private string _data = null!;

        // TODO: use
        private string? _dataTrimEndNewline;
        private string? _dataExtractedColorTrimEndNewline;
        private string? _newline;
        private bool _extractedColorsUpdated;

        public DataState(string data, bool stdout, Profile profile)
        {
            Data = data;
            Stdout = stdout;
            Profile = profile;
        }

        public DataState(Profile profile)
        {
            Profile = profile;
        }

        public ColorState GetColorState(int position)
        {
            return Profile.State.GetColorState(this, position);
        }
    }
}
