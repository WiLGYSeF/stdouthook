namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class AlignLeftFormatBuilder : AlignFormatBuilder
    {
        public override string? Key => "alignLeft";

        public override char? KeyShort => null;

        protected override string Align(string str, char c, int length)
        {
            return str + new string(c, length);
        }
    }
}
