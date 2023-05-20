using System;
using System.Collections.Generic;
using System.Text;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Formatters
{
    internal static class ColorExtractor
    {
        public static string ExtractColor(string data, IList<KeyValuePair<int, string>>? colors)
        {
            var length = data.Length;
            var builder = new StringBuilder(length);
            var dataSpan = data.AsSpan();
            var subtractOffset = 0;
            var last = 0;

            for (var i = 0; i < length; i++)
            {
                if (data[i] == 0x1b && i < length - 2 && data[i + 1] == '[')
                {
                    var colorIndex = i + 2;
                    for (; colorIndex < length; colorIndex++)
                    {
                        switch (data[colorIndex])
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
                            case ';':
                                break;
                            case 'm':
                                colorIndex++;
                                colors?.Add(new KeyValuePair<int, string>(i - subtractOffset, data[i..colorIndex]));
                                subtractOffset += colorIndex - i;

                                builder.Append(dataSpan[last..i]);
                                i = colorIndex;
                                last = i;
                                goto end;
                            default:
                                goto end;
                        }
                    }
                end:;
                }
            }

            return builder.Append(dataSpan[last..])
                .ToString();
        }

        public static void InsertExtractedColors(StringBuilder builder, ReadOnlySpan<char> input, int offset, IReadOnlyList<KeyValuePair<int, string>> colors)
        {
            var colorIndex = 0;
            for (; colorIndex < colors.Count && colors[colorIndex].Key < offset; colorIndex++) ;

            var last = 0;

            for (; colorIndex < colors.Count; colorIndex++)
            {
                var color = colors[colorIndex];
                var colorStart = color.Key - offset;

                if (colorStart > input.Length)
                {
                    break;
                }

                builder
                    .Append(input[last..colorStart])
                    .Append(color.Value);
                last = colorStart;
            }

            builder.Append(input[last..]);
        }

        public static void InsertExtractedColors(string[] splitData, IReadOnlyList<KeyValuePair<int, string>> colors)
        {
            var builder = new StringBuilder();
            var offset = 0;
            var colorIndex = 0;

            for (var i = 0; i < splitData.Length && colorIndex < colors.Count; i++)
            {
                var splitSpan = splitData[i].AsSpan();
                var endOffset = offset + splitSpan.Length;
                var isField = (i & 1) == 0;

                if (offset <= colors[colorIndex].Key
                    && IsColorPositionWithinMax(colors[colorIndex], endOffset, isField))
                {
                    splitData[i] = InsertExtractedColors(splitSpan, colors, builder, offset, endOffset, ref colorIndex, isField);
                }

                offset += splitSpan.Length;
            }
        }

        public static void InsertExtractedColors(StringBuilder builder, MatchGroup[] matches, IReadOnlyList<KeyValuePair<int, string>> colors)
        {
            var colorIndex = 0;

            for (var i = 0; i < matches.Length && colorIndex < colors.Count; i++)
            {
                var match = matches[i];
                var value = match.Value.AsSpan();
                var offset = match.Index;
                var endOffset = offset + value.Length;

                for (; colorIndex < colors.Count && colors[colorIndex].Key < match.Index; colorIndex++) ;

                if (offset <= colors[colorIndex].Key
                    && IsColorPositionWithinMax(colors[colorIndex], endOffset, true))
                {
                    var colorIndexCopy = colorIndex;
                    match.Value = InsertExtractedColors(value, colors, builder, offset, endOffset, ref colorIndexCopy, true);
                }
            }
        }

        private static string InsertExtractedColors(
            ReadOnlySpan<char> data,
            IReadOnlyList<KeyValuePair<int, string>> colors,
            StringBuilder builder,
            int offset,
            int endOffset,
            ref int colorIndex,
            bool isField)
        {
            var last = 0;

            builder.Clear();

            do
            {
                var delta = colors[colorIndex].Key - offset;
                builder
                    .Append(data[last..delta])
                    .Append(colors[colorIndex].Value);

                last = delta;
                colorIndex++;
            }
            while (colorIndex < colors.Count
                && IsColorPositionWithinMax(colors[colorIndex], endOffset, isField));

            return builder.Append(data[last..])
                .ToString();
        }

        private static bool IsColorPositionWithinMax(KeyValuePair<int, string> color, int max, bool isField)
        {
            return isField
                ? color.Key <= max
                : color.Key < max;
        }
    }
}
