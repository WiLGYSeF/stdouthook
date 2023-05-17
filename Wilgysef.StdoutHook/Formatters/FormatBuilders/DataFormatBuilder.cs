using System;
using Wilgysef.StdoutHook.Extensions;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class DataFormatBuilder : FormatBuilder
    {
        private const string NoTrim = "noTrim";

        private const string Newline = "newline";

        public override string? Key => "data";

        public override char? KeyShort => null;

        public override Func<DataState, string> Build(FormatBuildState state, out bool isConstant)
        {
            isConstant = false;

            if (state.Contents.Equals(NoTrim, StringComparison.OrdinalIgnoreCase))
            {
                return dataState => dataState.Data!;
            }
            else if (state.Contents.Equals(Newline, StringComparison.OrdinalIgnoreCase))
            {
                return dataState =>
                {
                    dataState.Data!.TrimEndNewline(out var newline);
                    return newline;
                };
            }
            else
            {
                return dataState => dataState.Data!.TrimEndNewline(out _);
            }
        }
    }
}
