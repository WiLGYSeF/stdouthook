using System;
using System.Text;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class ByteFormatBuilder : FormatBuilder
    {
        public override string? Key => null;

        public override char? KeyShort => 'x';

        public override Func<DataState, string> Build(FormatBuildState state, out bool isConstant)
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
            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'A':
                case 'B':
                case 'C':
                case 'E':
                case 'F':
                    return true;
                default:
                    return false;
            }
        }
    }
}
