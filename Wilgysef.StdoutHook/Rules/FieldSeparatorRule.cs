using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Extensions;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class FieldSeparatorRule : Rule
    {
        private const int MaximumFieldCount = 128;

        public Regex SeparatorRegex { get; set; }

        public int? MinFields { get; set; }

        public int? MaxFields { get; set; }

        public FieldRange? FieldCheck { get; set; }

        public Regex? FieldRegex { get; set; }

        public IList<KeyValuePair<FieldRange, string>> ReplaceFields { get; set; }

        private readonly List<KeyValuePair<FieldRange, string>> _outOfRangeReplaceFields = new List<KeyValuePair<FieldRange, string>>();

        private string?[]? _fieldReplacers;

        internal override void Build(Formatter formatter)
        {
            base.Build(formatter);

            var maxRange = GetMaximumNoninfiniteRange();
            var maxRangeClamped = Math.Min(maxRange, MaximumFieldCount);

            _fieldReplacers = new string[maxRangeClamped];

            for (var i = 0; i < maxRangeClamped; i++)
            {
                _fieldReplacers[i] = GetFirstRangeOrDefault(i + 1);
            }

            foreach (var kvp in ReplaceFields)
            {
                if (kvp.Key.Min > maxRangeClamped)
                {
                    _outOfRangeReplaceFields.Add(kvp);
                }
            }
        }

        internal override string Apply(string data, bool stdout, ProfileState state)
        {
            var splitData = SeparatorRegex.SplitWithSeparators(data, out var splitCount);

            if (MinFields.HasValue && MinFields.Value > splitCount
                || MaxFields.HasValue && MaxFields.Value < splitCount)
            {
                return data;
            }

            var limit = Math.Min(splitCount, _fieldReplacers!.Length);
            for (var i = 0; i < limit; i++)
            {
                var replace = _fieldReplacers[i];
                if (replace != null)
                {
                    splitData[i * 2] = Formatter.Format(replace);
                }
            }

            for (var i = limit; i < splitCount; i++)
            {
                foreach (var (range, replace) in _outOfRangeReplaceFields)
                {
                    if (range.Contains(i))
                    {
                        splitData[i * 2] = Formatter.Format(replace);
                        break;
                    }
                }
            }

            return string.Join("", splitData);
        }

        private string? GetFirstRangeOrDefault(int position)
        {
            foreach (var (range, replace) in ReplaceFields)
            {
                if (range.Contains(position))
                {
                    return replace;
                }
            }

            return null;
        }

        private int GetMaximumNoninfiniteRange()
        {
            var max = 0;

            foreach (var range in ReplaceFields)
            {
                if (range.Key.Max > max)
                {
                    max = range.Key.Max;
                }
            }

            return max;
        }
    }
}
