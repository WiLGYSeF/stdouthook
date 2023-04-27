using System.Collections.Generic;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Rules
{
    public abstract class Rule
    {
        public bool Enabled { get; set; } = true;

        public Regex? EnableRegex { get; set; }

        public bool StdoutOnly { get; set; }

        public bool StderrOnly { get; set; }

        public bool Terminal { get; set; }

        public IList<long> ActivationLines { get; set; } = new List<long>();

        public IList<long> ActivationLinesStdoutOnly { get; set; } = new List<long>();

        public IList<long> ActivationLinesStderrOnly { get; set; } = new List<long>();

        public IList<long> DeactivationLines { get; set; } = new List<long>();

        public IList<long> DeactivationLinesStdoutOnly { get; set; } = new List<long>();

        public IList<long> DeactivationLinesStderrOnly { get; set; } = new List<long>();

        public abstract void Build();

        public abstract string Apply(string data, bool stdout, ProfileState state);

        public virtual bool IsActive(bool stdout, ProfileState state)
        {
            if (!Enabled
                || StdoutOnly && !stdout
                || StderrOnly && stdout)
            {
                return false;
            }



            return true;
        }
    }
}
