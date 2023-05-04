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

        internal override void Build(ProfileState state, Formatter formatter)
        {
            base.Build(state, formatter);

            _compiledFormat = Formatter.CompileFormat(Format, state);
        }

        internal override string Apply(DataState state)
        {
            return _compiledFormat.Compute(state);
        }
    }
}
