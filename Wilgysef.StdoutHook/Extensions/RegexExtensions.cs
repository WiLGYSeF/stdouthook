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
                var isField = (i & 1) == 0;

                if (offset <= colors[colorIndex].Key
                    && IsColorPositionWithinMax(colors[colorIndex], endOffset, isField))
                {
                    splitData[i] = InsertExtractedColors(splitSpan, colors, builder, offset, endOffset, ref colorIndex, isField);
                }

                offset += splitSpan.Length;
            }

            return splitData;
        }

        public static MatchEntry[]? MatchExtractedColor(this Regex regex, string input)
        {
            var colors = new List<KeyValuePair<int, string>>();
            var data = ColorExtractor.ExtractColor(input, colors);

            var match = regex.Match(data);
            if (!match.Success)
            {
                return null;
            }

            var groups = new MatchEntry[match.Groups.Count];
            var builder = new StringBuilder();

            for (var i = 0; i < match.Groups.Count; i++)
            {
                var curMatch = match.Groups[i];
                var colorIndex = 0;

                var value = colorIndex < colors.Count
                    ? InsertExtractedColors(
                        curMatch.Value,
                        colors,
                        builder,
                        curMatch.Index,
                        curMatch.Value.Length,
                        ref colorIndex,
                        true)
                    : curMatch.Value;
                groups[i] = new MatchEntry(value, curMatch.Index);
            }

            return groups;
        }

        public class MatchEntry
        {
            public string Value { get; }

            public int Index { get; }

            public MatchEntry(string value, int index)
            {
                Value = value;
                Index = index;
            }
        }

        private static string InsertExtractedColors(
            ReadOnlySpan<char> data,
            List<KeyValuePair<int, string>> colors,
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
