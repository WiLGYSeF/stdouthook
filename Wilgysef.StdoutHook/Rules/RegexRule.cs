using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class RegexRule : Rule
    {
        public Regex Regex { get; set; }

        public override void Build()
        {

        }

        public override string Apply(string data, bool stdout, ProfileState state)
        {
            throw new System.NotImplementedException();
        }
    }
}
