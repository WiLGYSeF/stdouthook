using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Extensions;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;
using static Wilgysef.StdoutHook.Extensions.RegexExtensions;

namespace Wilgysef.StdoutHook.Rules
{
    public class RegexGroupRule : Rule
    {
        private static readonly int MaximumGroupCount = 16;

        public Regex Regex { get; set; }

        public IList<KeyValuePair<FieldRangeList, string>>? ReplaceGroups { get; set; }

        public string? ReplaceAllFormat { get; set; }

        private readonly List<KeyValuePair<FieldRangeList, CompiledFormat>> _outOfRangeReplaceGroups = new List<KeyValuePair<FieldRangeList, CompiledFormat>>();

        private CompiledFormat?[]? _groupReplacers;
        private CompiledFormat _compiledFormat = null!;

        public RegexGroupRule(Regex regex)
        {
            Regex = regex;
            ReplaceGroups = new List<KeyValuePair<FieldRangeList, string>>();
        }

        public RegexGroupRule(Regex regex, IList<KeyValuePair<FieldRangeList, string>>? replaceGroups)
        {
            Regex = regex;
            ReplaceGroups = replaceGroups;
        }

        public RegexGroupRule(Regex regex, string replaceAllFormat)
        {
            Regex = regex;
            ReplaceAllFormat = replaceAllFormat;
        }

        internal override void Build(ProfileState state, Formatter formatter)
        {
            base.Build(state, formatter);

            if (ReplaceGroups != null)
            {
                _groupReplacers = FieldRangeFormatCompiler.CompileFieldRangeFormats(
                    ReplaceGroups,
                    MaximumGroupCount,
                    _outOfRangeReplaceGroups,
                    format => Formatter.CompileFormat(format, state));
            }

            if (ReplaceAllFormat != null)
            {
                _compiledFormat = Formatter.CompileFormat(ReplaceAllFormat, state);
            }
        }

        internal override string Apply(DataState state)
        {
            var groups = Regex.MatchExtractedColor(state.Data!);
            if (groups == null)
            {
                return state.Data!;
            }

            using var contextScope = state.GetContextScope();
            var groupValues = new string[groups.Length];

            for (var i = 0; i < groups.Length; i++)
            {
                groupValues[i] = groups[i].Value;
            }

            state.Context.SetRegexGroupContext(groupValues);

            if (_compiledFormat != null)
            {
                return _compiledFormat.Compute(state);
            }

            var builder = new StringBuilder();
            var limit = Math.Min(groups.Length - 1, _groupReplacers!.Length);
            var last = 0;

            for (var i = 0; i < limit; i++)
            {
                AppendGroup(groups[i + 1], _groupReplacers[i]);
            }

            for (var i = limit; i < groups.Length - 1; i++)
            {
                CompiledFormat? foundReplace = null;

                foreach (var (rangeList, replace) in _outOfRangeReplaceGroups)
                {
                    if (rangeList.Contains(i))
                    {
                        foundReplace = replace;
                        break;
                    }
                }

                AppendGroup(groups[i + 1], foundReplace);
            }

            return builder.Append(state.Data![last..])
                .ToString();

            void AppendGroup(MatchEntry group, CompiledFormat? format)
            {
                if (group.Index >= last)
                {
                    builder
                        .Append(state.Data![last..group.Index])
                        .Append(format != null
                            ? format.Compute(state)
                            : group.Value);
                    last = group.Index + group.Value.Length;
                }
            }
        }
    }
}
