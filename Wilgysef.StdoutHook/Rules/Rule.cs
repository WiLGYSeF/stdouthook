using System.Collections.Generic;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Extensions;
using Wilgysef.StdoutHook.Formatters;
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

        public virtual bool Filter { get; protected set; }

        private protected Formatter Formatter { get; private set; } = null!;

        internal virtual void Build(ProfileState state, Formatter formatter)
        {
            Formatter = formatter;
        }

        internal abstract string Apply(DataState state);

        internal virtual bool IsActive(DataState state)
        {
            if (!Enabled
                || StdoutOnly && !state.Stdout
                || StderrOnly && state.Stdout
                || state.Data != null && EnableRegex != null && EnableRegex.MatchExtractedColor(state.Data.TrimEndNewline(out _)) == null)
            {
                return false;
            }

            // TODO: finish

            return true;
        }
    }
}
