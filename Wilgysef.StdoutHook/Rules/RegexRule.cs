using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Extensions;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class RegexRule : Rule
    {
        public Regex Regex { get; set; }

        public string ReplaceFormat { get; set; }

        private CompiledFormat _compiledFormat = null!;

        public RegexRule(Regex regex, string replaceFormat)
        {
            Regex = regex;
            ReplaceFormat = replaceFormat;
        }

        internal override void Build(ProfileState state, Formatter formatter)
        {
            base.Build(state,formatter);

            _compiledFormat = Formatter.CompileFormat(ReplaceFormat, state);
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

            return _compiledFormat.Compute(state);
        }
    }
}
