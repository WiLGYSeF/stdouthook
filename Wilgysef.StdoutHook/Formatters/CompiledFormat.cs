using System;
using System.Collections.Generic;
using System.Text;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters;

/// <summary>
/// Compiled format.
/// </summary>
internal class CompiledFormat
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompiledFormat"/> class.
    /// </summary>
    /// <remarks>
    /// The length of <paramref name="parts"/> must be one greater than the length of <paramref name="funcs"/>.
    /// </remarks>
    /// <param name="parts">String parts, joined around each format function.</param>
    /// <param name="funcs">Format functions.</param>
    internal CompiledFormat(
        IReadOnlyList<string> parts,
        IReadOnlyList<Func<FormatComputeState, string>> funcs)
    {
        Parts = parts;
        Funcs = funcs;
        IsConstant = Parts.Count == 1;
    }

    /// <summary>
    /// Indicates if the compiled format value is constant.
    /// </summary>
    public bool IsConstant { get; }

    /// <summary>
    /// Format string parts, joined around each format function.
    /// </summary>
    internal IReadOnlyList<string> Parts { get; }

    /// <summary>
    /// Format functions.
    /// </summary>
    internal IReadOnlyList<Func<FormatComputeState, string>> Funcs { get; }

    /// <summary>
    /// Computes the format.
    /// </summary>
    /// <param name="dataState">Data state.</param>
    /// <returns>Computed format.</returns>
    public string Compute(DataState dataState)
    {
        return Compute(dataState, 0);
    }

    /// <summary>
    /// Computes the format.
    /// </summary>
    /// <param name="dataState">Data state.</param>
    /// <param name="startPosition">Sets the start position offset.</param>
    /// <returns>Computed format.</returns>
    public string Compute(DataState dataState, int startPosition)
    {
        if (IsConstant)
        {
            return Parts[0];
        }

        var builder = new StringBuilder();
        var computeState = new FormatComputeState(dataState, startPosition);

        for (var i = 0; i < Funcs.Count; i++)
        {
            builder.Append(Parts[i]);

            computeState.SetPosition(builder.Length + startPosition);
            builder.Append(Funcs[i](computeState));
        }

        return builder.Append(Parts[^1])
            .ToString();
    }

    /// <summary>
    /// Copies the compiled format.
    /// </summary>
    /// <returns>Compiled format copy.</returns>
    public CompiledFormat Copy()
    {
        return new CompiledFormat(Parts, Funcs);
    }
}
