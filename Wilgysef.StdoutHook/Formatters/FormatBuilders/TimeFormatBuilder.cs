using System;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class TimeFormatBuilder : FormatBuilder
    {
        public override string? Key => "time";

        public override char? KeyShort => null;

        public override Func<DataState, string> Build(FormatBuildState state, out bool isConstant)
        {
            isConstant = false;
            var format = state.Contents;
            return _ => DateTime.Now.ToString(format);
        }
    }
}
