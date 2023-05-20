using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class RuleRegexGroupContext
    {
        public IReadOnlyDictionary<string, string> Groups { get; private set; }

        public int CurrentGroupNumber { get; set; } = 1;

        public bool IncrementGroupNumberOnGet { get; set; }

        public RuleRegexGroupContext(IReadOnlyDictionary<string, string> groups)
        {
            Groups = groups;
        }

        public int GetCurrentGroupNumber()
        {
            return IncrementGroupNumberOnGet
                ? CurrentGroupNumber++
                : CurrentGroupNumber;
        }

        public void Reset(IReadOnlyDictionary<string, string> groups)
        {
            Groups = groups;
            CurrentGroupNumber = 1;
            IncrementGroupNumberOnGet = false;
        }
    }
}
