namespace Wilgysef.StdoutHook.Extensions
{
    internal static class StringExtensions
    {
        public static string TrimEndNewline(this string str, out string newline)
        {
            var index = str.Length - 1;
            for (; index >= 0; index--)
            {
                if (str[index] != '\r' && str[index] != '\n')
                {
                    index++;
                    break;
                }
            }

            newline = str[index..];
            return str[..index];
        }
    }
}
