using System;
using System.Text;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Formatters
{
    internal static class ColorExtractor
    {
        public static string ExtractColor(string data, ColorList? colors)
        {
            var length = data.Length;
            var builder = new StringBuilder(length);
            var dataSpan = data.AsSpan();
            var subtractOffset = 0;
            var last = 0;

            for (var i = 0; i < length - 2; i++)
            {
                if (data[i] == 0x1b && data[i + 1] == '[')
                {
                    for (var colorIndex = i + 2; colorIndex < length; colorIndex++)
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
                                colors?.AddColor(i - subtractOffset, data, i, colorIndex);
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

        public static void InsertExtractedColors(StringBuilder builder, ReadOnlySpan<char> input, int offset, ColorList colors)
        {
            var colorIndex = colors.GetColorIndex(offset);
            var last = 0;

            for (; colorIndex < colors.Count; colorIndex++)
            {
                var color = colors[colorIndex];
                var colorStart = color.Position - offset;

                if (colorStart > input.Length)
                {
                    break;
                }

                builder
                    .Append(input[last..colorStart])
                    .Append(color.Color);
                last = colorStart;
            }

            builder.Append(input[last..]);
        }

        public static void InsertExtractedColors(string[] splitData, ColorList colors)
        {
            var builder = new StringBuilder();
            var offset = 0;
            var colorIndex = 0;

            for (var i = 0; i < splitData.Length && colorIndex < colors.Count; i++)
            {
                var splitSpan = splitData[i].AsSpan();
                var endOffset = offset + splitSpan.Length;
                var isField = (i & 1) == 0;

                if (offset <= colors[colorIndex].Position
                    && IsColorPositionWithinMax(colors[colorIndex].Position, endOffset, isField))
                {
                    splitData[i] = InsertExtractedColors(splitSpan, colors, builder, offset, endOffset, ref colorIndex, isField);
                }

                offset += splitSpan.Length;
            }
        }

        public static void InsertExtractedColors(StringBuilder builder, MatchGroup[] matches, ColorList colors)
        {
            var colorIndex = 0;

            for (var i = 0; i < matches.Length && colorIndex < colors.Count; i++)
            {
                var match = matches[i];
                var offset = match.Index;
                var endOffset = offset + match.Value.Length;

                for (; colorIndex < colors.Count && colors[colorIndex].Position < match.Index; colorIndex++) ;
                if (colorIndex == colors.Count)
                {
                    break;
                }

                if (offset <= colors[colorIndex].Position
                    && IsColorPositionWithinMax(colors[colorIndex].Position, endOffset, true))
                {
                    var colorIndexCopy = colorIndex;
                    match.Value = InsertExtractedColors(match.Value.AsSpan(), colors, builder, offset, endOffset, ref colorIndexCopy, true);
                }
            }
        }

        private static string InsertExtractedColors(
            ReadOnlySpan<char> data,
            ColorList colors,
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
                var delta = colors[colorIndex].Position - offset;
                builder
                    .Append(data[last..delta])
                    .Append(colors[colorIndex].Color);

                last = delta;
                colorIndex++;
            }
            while (colorIndex < colors.Count
                && IsColorPositionWithinMax(colors[colorIndex].Position, endOffset, isField));

            return builder.Append(data[last..])
                .ToString();
        }

        private static bool IsColorPositionWithinMax(int position, int max, bool isField)
        {
            return isField
                ? position <= max
                : position < max;
        }
    }
}
