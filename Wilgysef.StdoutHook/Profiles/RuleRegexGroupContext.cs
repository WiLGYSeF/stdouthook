using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class RuleRegexGroupContext
    {
        public IReadOnlyList<string> Groups { get; }

        public RuleRegexGroupContext(IReadOnlyList<string> groups)
        {
            Groups = new List<string>(groups);
        }
    }
}
