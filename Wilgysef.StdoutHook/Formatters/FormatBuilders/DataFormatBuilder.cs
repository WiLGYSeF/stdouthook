﻿using System;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

internal class DataFormatBuilder : FormatBuilder
{
    private const string NoTrim = "noTrim";

    private const string Newline = "newline";

    /// <inheritdoc/>
    public override string? Key => "data";

    /// <inheritdoc/>
    public override char? KeyShort => null;

    /// <inheritdoc/>
    public override Func<FormatComputeState, string> Build(FormatBuildState state, out bool isConstant)
    {
        isConstant = false;

        if (state.Contents.Equals(NoTrim, StringComparison.OrdinalIgnoreCase))
        {
            return computeState => computeState.DataState.Data;
        }
        else if (state.Contents.Equals(Newline, StringComparison.OrdinalIgnoreCase))
        {
            return computeState => computeState.DataState.Newline;
        }
        else
        {
            return computeState => computeState.DataState.DataTrimEndNewline;
        }
    }
}
