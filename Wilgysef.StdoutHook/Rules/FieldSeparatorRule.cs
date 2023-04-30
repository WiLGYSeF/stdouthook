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

        private readonly List<KeyValuePair<FieldRange, CompiledFormat>> _outOfRangeReplaceFields = new List<KeyValuePair<FieldRange, CompiledFormat>>();

        private CompiledFormat?[] _fieldReplacers = null!;

        internal override void Build(ProfileState state, Formatter formatter)
        {
            base.Build(state, formatter);

            var maxRange = GetMaximumNoninfiniteRange();
            var maxRangeClamped = Math.Min(maxRange, MaximumFieldCount);

            _fieldReplacers = new CompiledFormat[maxRangeClamped];

            for (var i = 0; i < maxRangeClamped; i++)
            {
                var value = GetFirstRangeOrDefault(i + 1);
                _fieldReplacers[i] = value != null
                    ? Formatter.CompileFormat(value, state)
                    : null;
            }

            foreach (var (range, replace) in ReplaceFields)
            {
                if (range.Min > maxRangeClamped)
                {
                    _outOfRangeReplaceFields.Add(new KeyValuePair<FieldRange, CompiledFormat>(
                        range,
                        Formatter.CompileFormat(replace, state)));
                }
            }
        }

        internal override string Apply(DataState state)
        {
            var splitData = SeparatorRegex.SplitWithSeparators(state.Data, out var splitCount);

            if (MinFields.HasValue && MinFields.Value > splitCount
                || MaxFields.HasValue && MaxFields.Value < splitCount)
            {
                return state.Data;
            }

            var limit = Math.Min(splitCount, _fieldReplacers!.Length);
            for (var i = 0; i < limit; i++)
            {
                var replace = _fieldReplacers[i];
                if (replace != null)
                {
                    splitData[i * 2] = replace.Compute(state);
                }
            }

            for (var i = limit; i < splitCount; i++)
            {
                foreach (var (range, replace) in _outOfRangeReplaceFields)
                {
                    if (range.Contains(i))
                    {
                        splitData[i * 2] = replace.Compute(state);
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
