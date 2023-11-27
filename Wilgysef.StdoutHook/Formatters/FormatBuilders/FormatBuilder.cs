using System;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

/// <summary>
/// Format builder.
/// </summary>
internal abstract class FormatBuilder
{
    /// <summary>
    /// Format key.
    /// </summary>
    public abstract string? Key { get; }

    /// <summary>
    /// Format short key.
    /// </summary>
    public abstract char? KeyShort { get; }

    /// <summary>
    /// Builds the format.
    /// </summary>
    /// <param name="state">Format build state.</param>
    /// <param name="isConstant">Indicates whether the built format method is a constant value.</param>
    /// <returns>The built formatter.</returns>
    public abstract Func<FormatComputeState, string> Build(FormatBuildState state, out bool isConstant);
}
