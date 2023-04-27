using System;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class UnconditionalReplaceRule : Rule
    {
        internal override void Build(Formatter formatter)
        {
            base.Build(formatter);
        }

        internal override string Apply(string data, bool stdout, ProfileState state)
        {
            return "test " + data;
        }
    }
}
