using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class RuleRegexGroupContext
    {
        public IReadOnlyList<string> Groups { get; }

        public int CurrentGroupNumber { get; set; } = 1;

        public bool IncrementGroupNumberOnGet { get; set; }

        public RuleRegexGroupContext(IReadOnlyList<string> groups)
        {
            Groups = new List<string>(groups);
        }

        public int GetCurrentGroupNumber()
        {
            return IncrementGroupNumberOnGet
                ? CurrentGroupNumber++
                : CurrentGroupNumber;
        }
    }
}
