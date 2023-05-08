using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class RuleRegexGroupContext
    {
        public IReadOnlyDictionary<string, string> Groups { get; }

        public int CurrentGroupNumber { get; set; } = 1;

        public bool IncrementGroupNumberOnGet { get; set; }

        public RuleRegexGroupContext(IReadOnlyDictionary<string, string> groups)
        {
            Groups = new Dictionary<string, string>(groups);
        }

        public int GetCurrentGroupNumber()
        {
            return IncrementGroupNumberOnGet
                ? CurrentGroupNumber++
                : CurrentGroupNumber;
        }
    }
}
