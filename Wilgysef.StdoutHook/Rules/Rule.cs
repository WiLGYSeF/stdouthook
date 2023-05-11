using System;
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

        protected SortedListIncrementMatch<long> _activationLines = null!;
        protected SortedListIncrementMatch<long> _activationLinesStdoutOnly = null!;
        protected SortedListIncrementMatch<long> _activationLinesStderrOnly = null!;
        protected SortedListIncrementMatch<long> _deactivationLines = null!;
        protected SortedListIncrementMatch<long> _deactivationLinesStdoutOnly = null!;
        protected SortedListIncrementMatch<long> _deactivationLinesStderrOnly = null!;

        protected bool _active = true;

        internal virtual void Build(ProfileState state, Formatter formatter)
        {
            Formatter = formatter;

            _activationLines = new SortedListIncrementMatch<long>(ActivationLines);
            _activationLinesStdoutOnly = new SortedListIncrementMatch<long>(ActivationLinesStdoutOnly);
            _activationLinesStderrOnly = new SortedListIncrementMatch<long>(ActivationLinesStderrOnly);
            _deactivationLines = new SortedListIncrementMatch<long>(DeactivationLines);
            _deactivationLinesStdoutOnly = new SortedListIncrementMatch<long>(DeactivationLinesStdoutOnly);
            _deactivationLinesStderrOnly = new SortedListIncrementMatch<long>(DeactivationLinesStderrOnly);
        }

        internal abstract string Apply(DataState state);

        internal virtual bool IsActive(DataState state)
        {
            if (!Enabled
                || StdoutOnly && !state.Stdout
                || StderrOnly && state.Stdout)
            {
                return false;
            }

            var profileState = state.ProfileState;
            var lineCount = profileState.LineCount;

            // bitwise or boolean operations to prevent short-circuiting
            if (_deactivationLines.MatchesCurrent(lineCount)
                | _deactivationLinesStdoutOnly.MatchesCurrent(profileState.StdoutLineCount)
                | _deactivationLinesStderrOnly.MatchesCurrent(profileState.StderrLineCount))
            {
                _active = false;
            }

            // activation takes priority over deactivation
            // bitwise or boolean operations to prevent short-circuiting
            if (_activationLines.MatchesCurrent(lineCount)
                | _activationLinesStdoutOnly.MatchesCurrent(profileState.StdoutLineCount)
                | _activationLinesStderrOnly.MatchesCurrent(profileState.StderrLineCount))
            {
                _active = true;
            }

            if (!_active)
            {
                return false;
            }

            if (state.Data != null && EnableRegex != null && EnableRegex.MatchExtractedColor(state.Data.TrimEndNewline(out _)) == null)
            {
                return false;
            }

            return true;
        }

        protected class SortedListIncrementMatch<T> where T : IEquatable<T>
        {
            private readonly List<T> _items;

            private int _index = 0;

            public SortedListIncrementMatch(IList<T> items)
            {
                _items = new List<T>(items);
                _items.Sort();
            }

            public bool MatchesCurrent(T item)
            {
                var originalIndex = _index;

                for (; _index < _items.Count && item.Equals(_items[_index]); _index++) { }

                return originalIndex != _index;
            }
        }
    }
}
