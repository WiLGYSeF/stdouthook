using System.Text.RegularExpressions;
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
            var match = Regex.Match(state.Data!);
            if (!match.Success)
            {
                return state.Data!;
            }

            return _compiledFormat.Compute(state);
        }
    }
}
