using System;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class ColorFormatBuilder : FormatBuilder
    {
        public override string? Key => "color";

        public override char? KeyShort => 'C';

        public override Func<string> Build(string format, out bool isConstant)
        {
            throw new NotImplementedException();
        }
    }
}
