using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class RuleFieldContext
    {
        public IReadOnlyList<string>? FieldsWithSeparators { get; private set; }

        public IReadOnlyList<string>? Fields { get; private set; }

        public IReadOnlyList<string>? FieldSeparators { get; private set; }

        public int HighestFieldNumber { get; set; }

        public RuleFieldContext(IReadOnlyList<string> fieldsWithSeparators)
        {
            var fieldsWithSeparatorsList = new List<string>(fieldsWithSeparators.Count);

            var fieldCount = fieldsWithSeparators.Count / 2 + 1;
            var fields = new List<string>(fieldCount);
            var fieldSeparators = new List<string>(fieldsWithSeparators.Count - fieldCount);

            FieldsWithSeparators = fieldsWithSeparatorsList;
            Fields = fields;
            FieldSeparators = fieldSeparators;

            var currentList = fields;
            foreach (var field in fieldsWithSeparators)
            {
                fieldsWithSeparatorsList.Add(field);
                currentList.Add(field);

                currentList = currentList == fields
                    ? fieldSeparators
                    : fields;
            }
        }
    }
}
