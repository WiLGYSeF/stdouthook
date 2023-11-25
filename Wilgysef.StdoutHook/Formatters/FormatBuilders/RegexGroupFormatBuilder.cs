using System;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

internal class RegexGroupFormatBuilder : FormatBuilder
{
    public override string? Key => "group";

    public override char? KeyShort => 'G';

    public override Func<FormatComputeState, string> Build(FormatBuildState state, out bool isConstant)
    {
        if (state.Contents.Length == 0)
        {
            throw new ArgumentException("Group must be specified.");
        }

        var contents = state.Contents;
        isConstant = false;

        if (contents.Equals("c", StringComparison.OrdinalIgnoreCase))
        {
            return computeState =>
            {
                var context = computeState.DataState.Context.RegexGroupContext;
                if (context == null)
                {
                    return "";
                }

                var groupNumber = context.GetCurrentGroupNumber();

                return context.Groups.TryGetValue(groupNumber.ToString(), out var value)
                    ? value
                    : "";
            };
        }

        return computeState =>
        {
            var context = computeState.DataState.Context.RegexGroupContext;
            return context != null && context.Groups.TryGetValue(contents, out var value)
                ? value
                : "";
        };
    }
}
