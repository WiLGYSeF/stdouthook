namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

internal class AlignRightFormatBuilder : AlignFormatBuilder
{
    /// <inheritdoc/>
    public override string? Key => "alignRight";

    /// <inheritdoc/>
    public override char? KeyShort => null;

    /// <inheritdoc/>
    protected override string Align(string str, char c, int length)
    {
        return new string(c, length) + str;
    }
}
