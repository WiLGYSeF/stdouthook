using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Extensions;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class RegexGroupRule : Rule
    {
        private static readonly int MaximumGroupCount = 32;

        public Regex Regex { get; set; }

        public IList<KeyValuePair<FieldRangeList, string>>? ReplaceGroups { get; set; }

        public IDictionary<string, string>? ReplaceNamedGroups { get; set; }

        private readonly List<KeyValuePair<FieldRangeList, CompiledFormat>> _outOfRangeReplaceGroups = new List<KeyValuePair<FieldRangeList, CompiledFormat>>();
        private readonly Dictionary<string, CompiledFormat> _namedGroups = new Dictionary<string, CompiledFormat>();

        private CompiledFormat?[]? _groupReplacers;

        public RegexGroupRule(Regex regex)
        {
            Regex = regex;
            ReplaceGroups = new List<KeyValuePair<FieldRangeList, string>>();
            ReplaceNamedGroups = new Dictionary<string, string>();
        }

        public RegexGroupRule(Regex regex, IList<KeyValuePair<FieldRangeList, string>> replaceGroups)
        {
            Regex = regex;
            ReplaceGroups = replaceGroups;
        }

        public RegexGroupRule(Regex regex, IDictionary<string, string> replaceNamedGroups)
        {
            Regex = regex;
            ReplaceNamedGroups = replaceNamedGroups;
        }

        public RegexGroupRule(
            Regex regex,
            IList<KeyValuePair<FieldRangeList, string>> replaceGroups,
            IDictionary<string, string> replaceNamedGroups)
        {
            Regex = regex;
            ReplaceGroups = replaceGroups;
            ReplaceNamedGroups = replaceNamedGroups;
        }

        internal override void Build(Profile profile, Formatter formatter)
        {
            base.Build(profile, formatter);

            if (ReplaceGroups != null)
            {
                _groupReplacers = FieldRangeFormatCompiler.CompileFieldRangeFormats(
                    ReplaceGroups,
                    MaximumGroupCount,
                    _outOfRangeReplaceGroups,
                    format => Formatter.CompileFormat(format, profile));
            }

            if (ReplaceNamedGroups != null)
            {
                foreach (var (name, format) in ReplaceNamedGroups)
                {
                    _namedGroups[name] = Formatter.CompileFormat(format, profile);
                }
            }
        }

        internal override string Apply(DataState state)
        {
            var data = state.DataExtractedColorTrimEndNewline.AsSpan();
            var matches = Regex.MatchAllGroups(state.DataExtractedColorTrimEndNewline);
            if (matches == null)
            {
                return state.Data;
            }

            var builder = new StringBuilder();
            var colorBuilder = new StringBuilder();
            var groupValues = new Dictionary<string, string>();
            var last = 0;

            for (var matchIndex = 0; matchIndex < matches.Count; matchIndex++)
            {
                var groups = matches[matchIndex];
                ColorExtractor.InsertExtractedColors(colorBuilder, groups, state.ExtractedColors);

                groupValues.Clear();

                for (var i = 0; i < groups.Length; i++)
                {
                    var group = groups[i];
                    groupValues[i.ToString()] = group.Value;
                    groupValues[group.Name] = group.Value;
                }

                state.Context.SetRegexGroupContext(groupValues);
                state.Context.RegexGroupContext!.IncrementGroupNumberOnGet = false;

                ColorExtractor.InsertExtractedColors(builder, data[last..groups[0].Index], last, state.ExtractedColors);
                last = groups[0].Index;

                var limit = Math.Min(groups.Length - 1, _groupReplacers?.Length ?? 0);

                for (var i = 0; i < limit; i++)
                {
                    AppendGroup(data, groups, i + 1, _groupReplacers![i]);
                }

                for (var i = limit; i < groups.Length - 1; i++)
                {
                    CompiledFormat? foundReplace = null;

                    foreach (var (rangeList, replace) in _outOfRangeReplaceGroups)
                    {
                        if (rangeList.Contains(i + 1))
                        {
                            foundReplace = replace;
                            break;
                        }
                    }

                    AppendGroup(data, groups, i + 1, foundReplace);
                }
            }

            ColorExtractor.InsertExtractedColors(builder, data[last..], last, state.ExtractedColors);

            if (!TrimNewline)
            {
                builder.Append(state.Newline);
            }

            return builder.ToString();

            void AppendGroup(ReadOnlySpan<char> span, MatchGroup[] groups, int groupNumber, CompiledFormat? format)
            {
                var group = groups[groupNumber];
                state.Context.RegexGroupContext!.CurrentGroupNumber = groupNumber;

                if (group.Index >= last)
                {
                    if (_namedGroups.TryGetValue(group.Name, out var namedFormat))
                    {
                        format = namedFormat;
                    }

                    ColorExtractor.InsertExtractedColors(builder, span[last..group.Index], last, state.ExtractedColors);
                    builder.Append(format != null
                        ? format.Compute(state)
                        : group.Value);
                    last = group.EndIndex;
                }
            }
        }
    }
}
