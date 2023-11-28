using System;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

internal class TimeFormatBuilder : FormatBuilder
{
    /// <inheritdoc/>
    public override string? Key => "time";

    /// <inheritdoc/>
    public override char? KeyShort => null;

    /// <inheritdoc/>
    public override Func<FormatComputeState, string> Build(FormatBuildState state, out bool isConstant)
    {
        isConstant = false;
        var format = state.Contents;
        return _ => DateTime.Now.ToString(format);
    }
}
