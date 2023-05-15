using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Extensions;

namespace Wilgysef.StdoutHook.Formatters
{
    internal static class ColorExtractor
    {
        private static readonly Regex ColorRegex = new Regex(@"\x1b\[([0-9;]+)m", RegexOptions.Compiled);

        public static string ExtractColor(string data, IList<KeyValuePair<int, string>>? colors)
        {
            var parts = ColorRegex.SplitWithSeparators(data, out var _);

            var builder = new StringBuilder(data.Length);
            var offset = 0;
            var useBuilder = true;

            for (var i = 0; i < parts.Length; i++)
            {
                if (useBuilder)
                {
                    builder.Append(parts[i]);
                    offset += parts[i].Length;
                    useBuilder = false;
                }
                else
                {
                    colors?.Add(new KeyValuePair<int, string>(offset, parts[i]));
                    useBuilder = true;
                }
            }

            return builder.ToString();
        }
    }
}
