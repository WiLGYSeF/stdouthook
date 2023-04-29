using Wilgysef.StdoutHook.Formatters.FormatBuilders;

namespace Wilgysef.StdoutHook.Tests;

internal class TestFormatBuilder : FormatBuilder
{
    public override string? Key => _key;

    public override char? KeyShort => _keyShort;

    private readonly string? _key;
    private readonly char? _keyShort;
    private readonly Func<string, string>? _func;
    private readonly Func<string, bool>? _constantFunc;

    public TestFormatBuilder(
        string? key,
        char? keyShort,
        Func<string, string>? func = null,
        Func<string, bool>? constantFunc = null)
    {
        _key = key;
        _keyShort = keyShort;
        _func = func;
        _constantFunc = constantFunc;
    }

    public override Func<string> Build(string format, out bool isConstant)
    {
        isConstant = _constantFunc != null && _constantFunc(format);
        return _func != null
            ? () => _func(format)
            : () => format;
    }
}
