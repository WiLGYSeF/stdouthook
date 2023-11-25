namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

internal class AlignRightFormatBuilder : AlignFormatBuilder
{
    public override string? Key => "alignRight";

    public override char? KeyShort => null;

    protected override string Align(string str, char c, int length)
    {
        return new string(c, length) + str;
    }
}
