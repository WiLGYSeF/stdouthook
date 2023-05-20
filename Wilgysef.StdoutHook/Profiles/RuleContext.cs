using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class RuleContext
    {
        public RuleFieldContext? FieldContext { get; private set; }

        public RuleRegexGroupContext? RegexGroupContext { get; private set; }

        public void SetFieldContext(IReadOnlyList<string> fieldsWithSeparators)
        {
            FieldContext = new RuleFieldContext(fieldsWithSeparators);
        }

        public void SetRegexGroupContext(IReadOnlyDictionary<string, string> groups)
        {
            RegexGroupContext = new RuleRegexGroupContext(groups);
        }

        public void Reset()
        {
            FieldContext = null;
            RegexGroupContext = null;
        }
    }
}
