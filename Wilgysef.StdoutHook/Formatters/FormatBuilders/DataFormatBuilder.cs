using System;
using Wilgysef.StdoutHook.Extensions;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class DataFormatBuilder : FormatBuilder
    {
        private const string NoTrim = "noTrim";

        public override string? Key => "data";

        public override char? KeyShort => null;

        public override Func<DataState, string> Build(FormatBuildState state, out bool isConstant)
        {
            isConstant = false;

            var noTrim = state.Contents.Equals(NoTrim, StringComparison.OrdinalIgnoreCase);
            return dataState => noTrim
                ? dataState.Data!
                : dataState.Data!.TrimEndNewline(out _);
        }
    }
}
