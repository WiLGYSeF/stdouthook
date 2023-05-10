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
        private static readonly int MaximumGroupCount = 32;

        public Regex Regex { get; set; }

        public IList<KeyValuePair<FieldRangeList, string>>? ReplaceGroups { get; set; }

        public IDictionary<string, string>? ReplaceNamedGroups { get; set; }

        public string? ReplaceAllFormat { get; set; }

        private readonly List<KeyValuePair<FieldRangeList, CompiledFormat>> _outOfRangeReplaceGroups = new List<KeyValuePair<FieldRangeList, CompiledFormat>>();
        private readonly Dictionary<string, CompiledFormat> _namedGroups = new Dictionary<string, CompiledFormat>();

        private CompiledFormat?[]? _groupReplacers;
        private CompiledFormat _compiledFormat = null!;

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

            if (ReplaceNamedGroups != null)
            {
                foreach (var (name, format) in ReplaceNamedGroups)
                {
                    _namedGroups[name] = Formatter.CompileFormat(format, state);
                }
            }

            if (ReplaceAllFormat != null)
            {
                _compiledFormat = Formatter.CompileFormat(ReplaceAllFormat, state);
            }
        }

        internal override string Apply(DataState state)
        {
            var data = state.Data!.TrimEndNewline(out var newline);
            var groups = Regex.MatchExtractedColor(data);
            if (groups == null)
            {
                return state.Data!;
            }

            using var contextScope = state.GetContextScope();
            var groupValues = new Dictionary<string, string>(groups.Length);

            for (var i = 0; i < groups.Length; i++)
            {
                var group = groups[i];
                groupValues[i.ToString()] = group.Value;
                groupValues[group.Name] = group.Value;
            }

            state.Context.SetRegexGroupContext(groupValues);

            if (_compiledFormat != null)
            {
                state.Context.RegexGroupContext!.IncrementGroupNumberOnGet = true;
                return _compiledFormat.Compute(state);
            }

            state.Context.RegexGroupContext!.IncrementGroupNumberOnGet = false;

            var builder = new StringBuilder();
            var limit = Math.Min(groups.Length - 1, _groupReplacers?.Length ?? 0);
            var last = 0;

            for (var i = 0; i < limit; i++)
            {
                AppendGroup(i + 1, _groupReplacers![i]);
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

                AppendGroup(i + 1, foundReplace);
            }

            return builder.Append(data[last..])
                .Append(newline)
                .ToString();

            void AppendGroup(int groupNumber, CompiledFormat? format)
            {
                var group = groups[groupNumber];
                state.Context.RegexGroupContext!.CurrentGroupNumber = groupNumber;

                if (group.Index >= last)
                {
                    if (_namedGroups.TryGetValue(group.Name, out var namedFormat))
                    {
                        format = namedFormat;
                    }

                    builder
                        .Append(data[last..group.Index])
                        .Append(format != null
                            ? format.Compute(state)
                            : group.Value);
                    last = group.Index + group.Value.Length;
                }
            }
        }
    }
}
