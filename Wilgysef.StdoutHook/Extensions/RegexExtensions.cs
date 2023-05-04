using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Formatters;

namespace Wilgysef.StdoutHook.Extensions
{
    internal static class RegexExtensions
    {
        public static string[] SplitWithSeparators(this Regex regex, string input, out int count)
        {
            var matchCollection = regex.Matches(input);
            var results = new string[matchCollection.Count * 2 + 1];
            var resultIndex = 0;
            var lastIndex = 0;

            for (var i = 0; i < matchCollection.Count; i++)
            {
                var match = matchCollection[i];
                results[resultIndex++] = input[lastIndex..match.Index];
                results[resultIndex++] = match.Value;
                lastIndex = match.Index + match.Value.Length;
            }

            results[resultIndex] = input[lastIndex..];
            count = results.Length / 2 + 1;
            return results;
        }

        public static string[] SplitWithSeparatorsExtractedColor(this Regex regex, string input, out int count)
        {
            var colors = new List<KeyValuePair<int, string>>();
            var data = ColorExtractor.ExtractColor(input, colors);

            var splitData = regex.SplitWithSeparators(data, out count);
            var builder = new StringBuilder();
            var offset = 0;
            var colorIndex = 0;

            for (var i = 0; i < splitData.Length && colorIndex < colors.Count; i++)
            {
                var splitSpan = splitData[i].AsSpan();
                var endOffset = offset + splitSpan.Length;

                if (offset <= colors[colorIndex].Key
                    && IsColorPositionWithinMax(colors[colorIndex], endOffset, (i & 1) == 0))
                {
                    var last = 0;

                    builder.Clear();

                    do
                    {
                        var delta = colors[colorIndex].Key - offset;
                        builder
                            .Append(splitSpan[last..delta])
                            .Append(colors[colorIndex].Value);

                        last = delta;
                        colorIndex++;
                    }
                    while (colorIndex < colors.Count
                        && IsColorPositionWithinMax(colors[colorIndex], endOffset, (i & 1) == 0));

                    splitData[i] = builder.Append(splitSpan[last..]).ToString();
                }

                offset += splitSpan.Length;
            }

            return splitData;

            static bool IsColorPositionWithinMax(KeyValuePair<int, string> color, int max, bool isField)
            {
                return isField
                    ? color.Key <= max
                    : color.Key < max;
            }
        }
    }
}
