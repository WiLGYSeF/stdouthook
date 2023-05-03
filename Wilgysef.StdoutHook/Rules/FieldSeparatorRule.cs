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
        private static readonly int MaximumFieldCount = 128;

        public Regex SeparatorRegex { get; set; }

        public int? MinFields { get; set; }

        public int? MaxFields { get; set; }

        public IList<KeyValuePair<FieldRangeList, string>>? ReplaceFields { get; set; }

        public string? ReplaceAllFormat { get; set; }

        private readonly List<KeyValuePair<FieldRangeList, CompiledFormat>> _outOfRangeReplaceFields = new List<KeyValuePair<FieldRangeList, CompiledFormat>>();

        private CompiledFormat?[]? _fieldReplacers;
        private CompiledFormat? _replaceAll;

        public FieldSeparatorRule(Regex separatorRegex)
        {
            SeparatorRegex = separatorRegex;
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>();
        }

        public FieldSeparatorRule(Regex separatorRegex, IList<KeyValuePair<FieldRangeList, string>> replaceFields)
        {
            SeparatorRegex = separatorRegex;
            ReplaceFields = replaceFields;
        }

        public FieldSeparatorRule(Regex separatorRegex, string replaceAllFormat)
        {
            SeparatorRegex = separatorRegex;
            ReplaceAllFormat = replaceAllFormat;
        }

        internal override void Build(ProfileState state, Formatter formatter)
        {
            base.Build(state, formatter);

            if (ReplaceAllFormat != null)
            {
                _replaceAll = Formatter.CompileFormat(ReplaceAllFormat, state);
                return;
            }

            var maxRange = GetMaximumRange();
            var maxRangeClamped = Math.Min(maxRange, MaximumFieldCount);

            _fieldReplacers = new CompiledFormat[maxRangeClamped];

            for (var i = 0; i < maxRangeClamped; i++)
            {
                var value = GetFirstRangeOrDefault(i + 1);
                _fieldReplacers[i] = value != null
                    ? Formatter.CompileFormat(value, state)
                    : null;
            }

            foreach (var (rangeList, replace) in ReplaceFields!)
            {
                if (rangeList.GetMin() > maxRangeClamped || rangeList.IsInfiniteMax())
                {
                    _outOfRangeReplaceFields.Add(new KeyValuePair<FieldRangeList, CompiledFormat>(
                        rangeList,
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

            using var contextScope = state.GetContextScope();
            state.Context.SetFieldContext(splitData);

            if (_replaceAll != null)
            {
                return _replaceAll.Compute(state);
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
                foreach (var (rangeList, replace) in _outOfRangeReplaceFields)
                {
                    if (rangeList.Contains(i))
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
            foreach (var (rangeList, replace) in ReplaceFields!)
            {
                if (rangeList.Contains(position))
                {
                    return replace;
                }
            }

            return null;
        }

        private int GetMaximumRange()
        {
            var max = 0;

            foreach (var (rangeList, _) in ReplaceFields!)
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
