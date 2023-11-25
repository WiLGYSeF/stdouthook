using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class RuleRegexGroupContext
    {
        public RuleRegexGroupContext(IReadOnlyDictionary<string, string> groups)
        {
            Groups = groups;
        }

        public IReadOnlyDictionary<string, string> Groups { get; private set; }

        public int CurrentGroupNumber { get; set; } = 1;

        public bool IncrementGroupNumberOnGet { get; set; }

        public int GetCurrentGroupNumber()
        {
#pragma warning disable SA1003 // Symbols should be spaced correctly
            return IncrementGroupNumberOnGet
                ? CurrentGroupNumber++
                : CurrentGroupNumber;
#pragma warning restore SA1003 // Symbols should be spaced correctly
        }

        public void Reset(IReadOnlyDictionary<string, string> groups)
        {
            Groups = groups;
            CurrentGroupNumber = 1;
            IncrementGroupNumberOnGet = false;
        }
    }
}
