using System;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class UnconditionalReplaceRule : Rule
    {
        public override void Build()
        {

        }

        public override string Apply(string data, bool stdout, ProfileState state)
        {
            return "test " + data;
        }
    }
}
