using System;
using Wilgysef.StdoutHook.Extensions;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

internal class SubstringFormatBuilder : FormatBuilder
{
    public override string? Key => "substring";

    public override char? KeyShort => null;

    public override Func<FormatComputeState, string> Build(FormatBuildState state, out bool isConstant)
    {
        var contentsSpan = state.Contents.AsSpan();
        var separatorIndex = contentsSpan.IndexOf(Formatter.Separator);

        if (separatorIndex < 1 || !int.TryParse(contentsSpan[..separatorIndex], out var startIndex))
        {
            throw new ArgumentException($"Invalid substring format: {state.Contents}");
        }

        int? endIndex = null;

        var secondSeparatorIndex = contentsSpan.IndexOfAfter(separatorIndex + 1, Formatter.Separator);
        if (secondSeparatorIndex >= 0
            && int.TryParse(contentsSpan[(separatorIndex + 1)..secondSeparatorIndex], out var _endIndex))
        {
            endIndex = _endIndex;
        }
        else
        {
            secondSeparatorIndex = separatorIndex;
        }

        var format = state.Profile.CompileFormat(contentsSpan[(secondSeparatorIndex + 1)..].ToString());

        isConstant = format.IsConstant;
        return computeState =>
        {
            var result = format.Compute(computeState.DataState, computeState.Position);
            var start = startIndex >= 0 ? startIndex : result.Length + startIndex;
            var end = Math.Min(
                endIndex.HasValue
                    ? (endIndex.Value >= 0 ? endIndex.Value : result.Length + endIndex.Value)
                    : result.Length,
                result.Length);

            return start < result.Length
                ? result[start..end]
                : "";
        };
    }
}
