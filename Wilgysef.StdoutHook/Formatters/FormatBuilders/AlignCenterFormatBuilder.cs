namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

internal class AlignCenterFormatBuilder : AlignFormatBuilder
{
    /// <inheritdoc/>
    public override string? Key => "alignCenter";

    /// <inheritdoc/>
    public override char? KeyShort => null;

    /// <inheritdoc/>
    protected override string Align(string str, char c, int length)
    {
        var halfLength = length / 2;
        return new string(c, halfLength) + str + new string(c, length - halfLength);
    }
}
