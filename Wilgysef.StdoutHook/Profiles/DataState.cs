using Wilgysef.StdoutHook.Extensions;
using Wilgysef.StdoutHook.Formatters;

namespace Wilgysef.StdoutHook.Profiles;

internal class DataState
{
    private readonly ColorList _extractedColors = new();
    private string _data = null!;

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

    public string Data
    {
        get => _data;
        internal set
        {
            if (_data != value)
            {
                _data = value;
                _dataTrimEndNewline = null;
                _dataExtractedColorTrimEndNewline = null;
                _extractedColors.Clear();
                _extractedColorsUpdated = false;
            }
        }
    }

    public string DataTrimEndNewline
    {
        get
        {
            _dataTrimEndNewline ??= _data.TrimEndNewline(out _newline);
            return _dataTrimEndNewline;
        }
    }

    public string DataExtractedColorTrimEndNewline
    {
        get
        {
            if (_dataExtractedColorTrimEndNewline == null)
            {
                ExtractColors();
            }

            return _dataExtractedColorTrimEndNewline!;
        }
    }

    public string Newline
    {
        get
        {
            if (_newline == null)
            {
                _dataTrimEndNewline = _data.TrimEndNewline(out _newline);
            }

            return _newline;
        }
    }

    public ColorList ExtractedColors
    {
        get
        {
            if (!_extractedColorsUpdated)
            {
                ExtractColors();
            }

            return _extractedColors;
        }
    }

    public bool Stdout { get; }

    public Profile Profile { get; }

    public RuleContext Context { get; } = new();

    internal int LastColorFormatIndex { get; set; } = -1;

    public ColorState GetColorState(int position)
    {
        return Profile.State.GetColorState(this, position);
    }

    private void ExtractColors()
    {
        _dataExtractedColorTrimEndNewline = ColorExtractor.ExtractColor(DataTrimEndNewline, _extractedColors);
        _extractedColorsUpdated = true;
    }
}
