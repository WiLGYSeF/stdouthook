using System;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class LengthFormatBuilder : FormatBuilder
    {
        public override string? Key => "length";

        public override char? KeyShort => null;

        public override Func<DataState, string> Build(FormatBuildState state, out bool isConstant)
        {
            var format = state.Profile.CompileFormat(state.Contents);

            isConstant = format.IsConstant;
            return dataState => format.Compute(dataState).Length.ToString();
        }
    }
}
