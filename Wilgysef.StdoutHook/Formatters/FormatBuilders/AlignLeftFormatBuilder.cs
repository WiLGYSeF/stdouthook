namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

internal class AlignLeftFormatBuilder : AlignFormatBuilder
{
    /// <inheritdoc/>
    public override string? Key => "alignLeft";

    /// <inheritdoc/>
    public override char? KeyShort => null;

    /// <inheritdoc/>
    protected override string Align(string str, char c, int length)
    {
        return str + new string(c, length);
    }
}
