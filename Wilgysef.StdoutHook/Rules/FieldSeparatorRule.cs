using System.Collections.Generic;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public class FieldSeparatorRule : Rule
    {
        public Regex SeparatorRegex { get; set; }

        public int? MinFields { get; set; }

        public int? MaxFields { get; set; }

        public FieldRange? FieldCheck { get; set; }

        public Regex? FieldRegex { get; set; }

        public IList<KeyValuePair<FieldRange, string>> ReplaceFields { get; } = new List<KeyValuePair<FieldRange, string>>();

        public override string Apply(string data, bool stdout, ProfileState state)
        {
            throw new System.NotImplementedException();
        }
    }
}
