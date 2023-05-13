using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class RuleFieldContext
    {
        public IReadOnlyList<string> Fields { get; }

        public IReadOnlyList<string> FieldSeparators { get; }

        public int CurrentFieldNumber { get; set; } = 1;

        public bool IncrementFieldNumberOnGet { get; set; }

        public RuleFieldContext(IReadOnlyList<string> fieldsWithSeparators)
        {
            var fieldCount = fieldsWithSeparators.Count / 2 + 1;
            var fields = new string[fieldCount];
            var fieldSeparators = new string[fieldsWithSeparators.Count - fieldCount];

            Fields = fields;
            FieldSeparators = fieldSeparators;

            var fieldIndex = 0;
            var fieldSeparatorIndex = 0;

            for (var i = 0; i < fieldsWithSeparators.Count; i++)
            {
                if ((i & 1) == 0)
                {
                    fields[fieldIndex++] = fieldsWithSeparators[i];
                }
                else
                {
                    fieldSeparators[fieldSeparatorIndex++] = fieldsWithSeparators[i];
                }
            }
        }

        public int GetCurrentFieldNumber()
        {
            return IncrementFieldNumberOnGet
                ? CurrentFieldNumber++
                : CurrentFieldNumber;
        }
    }
}
