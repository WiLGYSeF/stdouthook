using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles
{
    public class Profile
    {
        public IReadOnlyList<Rule> Rules => _rules;

        private readonly List<Rule> _rules = new List<Rule>();
    }
}
