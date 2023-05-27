using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Formatters.FormatBuilders;

namespace Wilgysef.StdoutHook.Tests;

internal class TestFormatBuilder : FormatBuilder
{
    public override string? Key => _key;

    public override char? KeyShort => _keyShort;

    private readonly string? _key;
    private readonly char? _keyShort;
    private readonly Func<FormatComputeState, string>? _func;
    private readonly Func<FormatBuildState, bool>? _constantFunc;

    public TestFormatBuilder(
        string? key,
        char? keyShort,
        Func<FormatComputeState, string>? func = null,
        Func<FormatBuildState, bool>? constantFunc = null)
    {
        _key = key;
        _keyShort = keyShort;
        _func = func;
        _constantFunc = constantFunc;
    }

    public override Func<FormatComputeState, string> Build(FormatBuildState state, out bool isConstant)
    {
        isConstant = _constantFunc != null && _constantFunc(state);
        return _func ?? (_ => state.Contents);
    }
}
