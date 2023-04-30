using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class RegexRule : Rule
    {
        public Regex Regex { get; set; }

        internal override void Build(ProfileState state, Formatter formatter)
        {
            base.Build(state,formatter);
        }

        internal override string Apply(DataState state)
        {
            throw new System.NotImplementedException();
        }
    }
}
