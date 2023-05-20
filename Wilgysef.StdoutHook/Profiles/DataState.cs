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

        public RuleContext Context { get; } = new RuleContext();

        private readonly ColorList _extractedColors = new ColorList();
        private string _data = null!;
        private string _newline = null!;

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
    }
}
