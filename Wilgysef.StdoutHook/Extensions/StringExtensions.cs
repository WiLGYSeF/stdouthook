namespace Wilgysef.StdoutHook.Extensions
{
    internal static class StringExtensions
    {
        public static string TrimEndNewline(this string str, out string newline)
        {
            if (str.Length == 0)
            {
                newline = "";
                return "";
            }

            var index = str.Length - 1;
            for (; index >= 0; index--)
            {
                if (str[index] != '\r' && str[index] != '\n')
                {
                    index++;
                    break;
                }
            }

            if (index == str.Length)
            {
                newline = "";
                return str;
            }

            newline = str[index..];
            return str[..index];
        }
    }
}
