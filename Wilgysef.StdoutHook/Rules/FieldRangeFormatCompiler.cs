using System;
using System.Collections.Generic;
using Wilgysef.StdoutHook.Formatters;

namespace Wilgysef.StdoutHook.Rules
{
    internal static class FieldRangeFormatCompiler
    {
        public static CompiledFormat?[] CompileFieldRangeFormats(
            IList<KeyValuePair<FieldRangeList, string>> fieldRanges,
            int maxCompiledFields,
            IList<KeyValuePair<FieldRangeList, CompiledFormat>> outOfRangeFields,
            Func<string, CompiledFormat> compileFormat)
        {
            var maxRange = GetMaximumRange(fieldRanges);
            var maxRangeClamped = Math.Min(maxRange, maxCompiledFields);

            var compiledFields = new CompiledFormat?[maxRangeClamped];

            for (var i = 0; i < maxRangeClamped; i++)
            {
                var value = GetFirstRangeOrDefault(fieldRanges, i + 1);
                compiledFields[i] = value != null
                    ? compileFormat(value)
                    : null;
            }

            foreach (var (rangeList, replace) in fieldRanges)
            {
                if (rangeList.GetMin() > maxRangeClamped || rangeList.IsInfiniteMax())
                {
                    outOfRangeFields.Add(new KeyValuePair<FieldRangeList, CompiledFormat>(
                        rangeList,
                        compileFormat(replace)));
                }
            }

            return compiledFields;
        }

        private static string? GetFirstRangeOrDefault(
            IList<KeyValuePair<FieldRangeList, string>> fieldRanges,
            int position)
        {
            foreach (var (rangeList, replace) in fieldRanges)
            {
                if (rangeList.Contains(position))
                {
                    return replace;
                }
            }

            return null;
        }

        private static int GetMaximumRange(IList<KeyValuePair<FieldRangeList, string>> fieldRanges)
        {
            var max = 0;

            foreach (var (rangeList, _) in fieldRanges)
            {
                if (rangeList.IsInfiniteMax())
                {
                    return int.MaxValue;
                }

                var curMax = rangeList.GetMax();
                if (curMax > max)
                {
                    max = curMax;
                }
            }

            return max;
        }
    }
}
