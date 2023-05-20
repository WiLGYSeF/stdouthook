namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class AlignCenterFormatBuilder : AlignFormatBuilder
    {
        public override string? Key => "alignCenter";

        public override char? KeyShort => null;

        protected override string Align(string str, char c, int length)
        {
            var halfLength = length / 2;
            return new string(c, halfLength) + str + new string(c, length - halfLength);
        }
    }
}
