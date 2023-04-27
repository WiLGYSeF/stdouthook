using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class RegexRule : Rule
    {
        public Regex Regex { get; set; }

        internal override void Build(Formatter formatter)
        {
            base.Build(formatter);
        }

        internal override string Apply(string data, bool stdout, ProfileState state)
        {
            throw new System.NotImplementedException();
        }
    }
}
