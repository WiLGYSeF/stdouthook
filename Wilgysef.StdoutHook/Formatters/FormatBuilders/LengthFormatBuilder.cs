using System;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

internal class LengthFormatBuilder : FormatBuilder
{
    /// <inheritdoc/>
    public override string? Key => "length";

    /// <inheritdoc/>
    public override char? KeyShort => null;

    /// <inheritdoc/>
    public override Func<FormatComputeState, string> Build(FormatBuildState state, out bool isConstant)
    {
        var format = state.Profile.CompileFormat(state.Contents);

        isConstant = format.IsConstant;
        return computeState => format.Compute(computeState.DataState, computeState.Position).Length.ToString();
    }
}
