using System;
using System.Collections.Generic;
using System.Text;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal abstract class FormatBuilder
    {
        public abstract string Key { get; }

        public abstract char KeyShort { get; }

        public abstract Func<string> Build(string format, out bool isConstant);
    }
}
