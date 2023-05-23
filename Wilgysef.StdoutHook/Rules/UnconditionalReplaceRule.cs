using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class UnconditionalReplaceRule : Rule
    {
        public string Format { get; set; }

        private CompiledFormat _compiledFormat = null!;

        public UnconditionalReplaceRule(string format)
        {
            Format = format;
        }

        internal override void Build(Profile profile, Formatter formatter)
        {
            base.Build(profile, formatter);

            _compiledFormat = Formatter.CompileFormat(Format, profile);
        }

        internal override string Apply(DataState state)
        {
            var result = _compiledFormat.Compute(state);

            return TrimNewline
                ? result
                : result + state.Newline;
        }

        protected override Rule CopyInternal()
        {
            return new UnconditionalReplaceRule(Format)
            {
                _compiledFormat = _compiledFormat?.Copy()!,
            };
        }
    }
}
