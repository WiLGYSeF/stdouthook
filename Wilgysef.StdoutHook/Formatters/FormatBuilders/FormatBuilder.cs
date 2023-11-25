using System;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

internal abstract class FormatBuilder
{
    public abstract string? Key { get; }

    public abstract char? KeyShort { get; }

    public abstract Func<FormatComputeState, string> Build(FormatBuildState state, out bool isConstant);
}
