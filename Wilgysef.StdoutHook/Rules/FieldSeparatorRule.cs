using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Extensions;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class FieldSeparatorRule : Rule
    {
        private static readonly int MaximumFieldCount = 128;

        public Regex SeparatorExpression { get; set; }

        public int? MinFields { get; set; }

        public int? MaxFields { get; set; }

        public IList<KeyValuePair<FieldRangeList, string>>? ReplaceFields { get; set; }

        public string? ReplaceAllFormat { get; set; }

        private readonly List<KeyValuePair<FieldRangeList, CompiledFormat>> _outOfRangeReplaceFields = new List<KeyValuePair<FieldRangeList, CompiledFormat>>();

        // TODO: optimize to sparse?
        private CompiledFormat?[]? _fieldReplacers;
        private CompiledFormat? _replaceAll;

        public FieldSeparatorRule(Regex separatorRegex)
        {
            SeparatorExpression = separatorRegex;
            ReplaceFields = new List<KeyValuePair<FieldRangeList, string>>();
        }

        public FieldSeparatorRule(Regex separatorRegex, IList<KeyValuePair<FieldRangeList, string>> replaceFields)
        {
            SeparatorExpression = separatorRegex;
            ReplaceFields = replaceFields;
        }

        public FieldSeparatorRule(Regex separatorRegex, string replaceAllFormat)
        {
            SeparatorExpression = separatorRegex;
            ReplaceAllFormat = replaceAllFormat;
        }

        internal override void Build(Profile profile, Formatter formatter)
        {
            base.Build(profile, formatter);

            if (ReplaceAllFormat != null && ReplaceFields != null && ReplaceFields.Count > 0)
            {
                throw new Exception($"Cannot have {nameof(ReplaceAllFormat)} and {nameof(ReplaceFields)} set.");
            }

            if (ReplaceAllFormat != null)
            {
                _replaceAll = Formatter.CompileFormat(ReplaceAllFormat, profile);
                return;
            }

            _fieldReplacers = FieldRangeFormatCompiler.CompileFieldRangeFormats(
                ReplaceFields!,
                MaximumFieldCount,
                _outOfRangeReplaceFields,
                format => Formatter.CompileFormat(format, profile));
        }

        internal override string Apply(DataState state)
        {
            var splitData = SeparatorExpression.SplitWithSeparators(
                state.DataExtractedColorTrimEndNewline,
                out var splitCount);

            if (MinFields.HasValue && MinFields.Value > splitCount
                || MaxFields.HasValue && MaxFields.Value < splitCount)
            {
                return state.Data;
            }

            ColorExtractor.InsertExtractedColors(splitData, state.ExtractedColors);
            state.Context.SetFieldContext(splitData);

            if (_replaceAll != null)
            {
                state.Context.FieldContext!.IncrementFieldNumberOnGet = true;
                return _replaceAll.Compute(state);
            }

            state.Context.FieldContext!.IncrementFieldNumberOnGet = false;

            var builder = new StringBuilder();
            var limit = Math.Min(splitCount, _fieldReplacers!.Length);
            var index = 0;

            for (var i = 0; i < limit; i++)
            {
                var replace = _fieldReplacers[i];
                if (replace != null)
                {
                    state.Context.FieldContext.CurrentFieldNumber = i + 1;
                    builder.Append(replace.Compute(state));
                    index++;
                }
                else
                {
                    builder.Append(splitData[index++]);
                }

                if (index < splitData.Length)
                {
                    builder.Append(splitData[index++]);
                }
            }

            for (var i = limit; i < splitCount; i++)
            {
                CompiledFormat? foundReplace = null;
                foreach (var (rangeList, replace) in _outOfRangeReplaceFields)
                {
                    if (rangeList.Contains(i))
                    {
                        foundReplace = replace;
                        break;
                    }
                }

                if (foundReplace != null)
                {
                    state.Context.FieldContext.CurrentFieldNumber = i + 1;
                    builder.Append(foundReplace.Compute(state));
                    index++;
                }
                else
                {
                    builder.Append(splitData[index++]);
                }

                if (index < splitData.Length)
                {
                    builder.Append(splitData[index++]);
                }
            }

            if (!TrimNewline)
            {
                builder.Append(state.Newline);
            }

            return builder.ToString();
        }

        protected override Rule CopyInternal()
        {
            var rule = new FieldSeparatorRule(SeparatorExpression)
            {
                MinFields = MinFields,
                MaxFields = MaxFields,
                ReplaceFields = ReplaceFields,
                ReplaceAllFormat = ReplaceAllFormat
            };

            for (var i = 0; i < _outOfRangeReplaceFields.Count; i++)
            {
                var (rangeList, replace) = _outOfRangeReplaceFields[i];
                rule._outOfRangeReplaceFields.Add(new KeyValuePair<FieldRangeList, CompiledFormat>(rangeList, replace.Copy()));
            }

            if (_fieldReplacers != null)
            {
                rule._fieldReplacers = new CompiledFormat?[_fieldReplacers.Length];
                for (var i = 0; i < _fieldReplacers.Length; i++)
                {
                    rule._fieldReplacers[i] = _fieldReplacers[i]?.Copy();
                }
            }

            rule._replaceAll = _replaceAll?.Copy();
            return rule;
        }
    }
}
