using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Wilgysef.StdoutHook.Profiles
{
    public abstract class Rule
    {
        public bool Enabled { get; set; }

        public Regex? EnableRegex { get; set; }

        public bool StdoutOnly { get; set; }

        public bool StderrOnly { get; set; }

        public bool Terminal { get; set; }

        public IList<int> ActivationLines { get; set; } = new List<int>();

        public IList<int> DeactivationLines { get; set; } = new List<int>();
    }
}
