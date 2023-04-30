using System;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal abstract class FormatBuilder
    {
        public abstract string? Key { get; }

        public abstract char? KeyShort { get; }

        public abstract Func<DataState, string> Build(FormatBuildState state, out bool isConstant);
    }
}
