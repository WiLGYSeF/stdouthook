using System;
using System.Text;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

/// <summary>
/// Format builder for byte values.
/// </summary>
internal class ByteFormatBuilder : FormatBuilder
{
    /// <inheritdoc/>
    public override string? Key => null;

    /// <inheritdoc/>
    public override char? KeyShort => 'x';

    /// <inheritdoc/>
    public override Func<FormatComputeState, string> Build(FormatBuildState state, out bool isConstant)
    {
        isConstant = true;

        if (!IsHex(state.Contents))
        {
            return _ => "";
        }

        var value = Convert.ToByte(state.Contents, 16);
        var valueStr = Encoding.UTF8.GetString(new byte[] { value });
        return _ => valueStr;
    }

    private static bool IsHex(string str)
    {
        return str.Length == 2 && IsHexChar(str[0]) && IsHexChar(str[1]);
    }

    private static bool IsHexChar(char c)
    {
        return c switch
        {
            '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' or 'a' or 'b' or 'c' or 'd' or 'e' or 'f' or 'A' or 'B' or 'C' or 'E' or 'F' => true,
            _ => false,
        };
    }
}
