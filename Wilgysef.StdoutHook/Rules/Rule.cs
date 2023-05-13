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

        public Regex? EnableExpression { get; set; }

        public bool StdoutOnly { get; set; }

        public bool StderrOnly { get; set; }

        public bool Terminal { get; set; }

        public bool TrimNewline { get; set; }

        public ICollection<long> ActivationLines { get; set; } = new List<long>();

        public ICollection<long> ActivationLinesStdoutOnly { get; set; } = new List<long>();

        public ICollection<long> ActivationLinesStderrOnly { get; set; } = new List<long>();

        public ICollection<long> DeactivationLines { get; set; } = new List<long>();

        public ICollection<long> DeactivationLinesStdoutOnly { get; set; } = new List<long>();

        public ICollection<long> DeactivationLinesStderrOnly { get; set; } = new List<long>();

        public ICollection<ActivationExpression> ActivationExpressions { get; set; } = new List<ActivationExpression>();

        public ICollection<ActivationExpression> ActivationExpressionsStdoutOnly { get; set; } = new List<ActivationExpression>();

        public ICollection<ActivationExpression> ActivationExpressionsStderrOnly { get; set; } = new List<ActivationExpression>();

        public ICollection<ActivationExpression> DeactivationExpressions { get; set; } = new List<ActivationExpression>();

        public ICollection<ActivationExpression> DeactivationExpressionsStdoutOnly { get; set; } = new List<ActivationExpression>();

        public ICollection<ActivationExpression> DeactivationExpressionsStderrOnly { get; set; } = new List<ActivationExpression>();

        public virtual bool Filter { get; protected set; }

        private protected Formatter Formatter { get; private set; } = null!;

        protected SortedListIncrementMatch<long> _activationLines = null!;
        protected SortedListIncrementMatch<long> _activationLinesStdoutOnly = null!;
        protected SortedListIncrementMatch<long> _activationLinesStderrOnly = null!;
        protected SortedListIncrementMatch<long> _deactivationLines = null!;
        protected SortedListIncrementMatch<long> _deactivationLinesStdoutOnly = null!;
        protected SortedListIncrementMatch<long> _deactivationLinesStderrOnly = null!;

        protected HashSet<long> _activationOffsetLines = new HashSet<long>();
        protected HashSet<long> _activationOffsetStdoutOnlyLines = new HashSet<long>();
        protected HashSet<long> _activationOffsetStderrOnlyLines = new HashSet<long>();
        protected HashSet<long> _deactivationOffsetLines = new HashSet<long>();
        protected HashSet<long> _deactivationOffsetStdoutOnlyLines = new HashSet<long>();
        protected HashSet<long> _deactivationOffsetStderrOnlyLines = new HashSet<long>();

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

            // avoid potential race conditions
            var lineCount = profileState.LineCount;
            var stdoutLineCount = profileState.StdoutLineCount;
            var stderrLineCount = profileState.StderrLineCount;

            var data = state.Data?.TrimEndNewline(out var newline);

            if (data != null)
            {
                MatchExpressions(ActivationExpressions, _activationOffsetLines, lineCount, data);
                MatchExpressions(ActivationExpressionsStdoutOnly, _activationOffsetStdoutOnlyLines, stdoutLineCount, data);
                MatchExpressions(ActivationExpressionsStderrOnly, _activationOffsetStderrOnlyLines, stderrLineCount, data);
                MatchExpressions(DeactivationExpressions, _deactivationOffsetLines, lineCount, data);
                MatchExpressions(DeactivationExpressionsStdoutOnly, _deactivationOffsetStdoutOnlyLines, stdoutLineCount, data);
                MatchExpressions(DeactivationExpressionsStderrOnly, _deactivationOffsetStderrOnlyLines, stderrLineCount, data);
            }

            // bitwise or boolean operations to prevent short-circuiting
            if (_deactivationLines.MatchesCurrent(lineCount)
                | _deactivationLinesStdoutOnly.MatchesCurrent(stdoutLineCount)
                | _deactivationLinesStderrOnly.MatchesCurrent(stderrLineCount)
                | _deactivationOffsetLines.Remove(lineCount)
                | _deactivationOffsetStdoutOnlyLines.Remove(stdoutLineCount)
                | _deactivationOffsetStderrOnlyLines.Remove(stderrLineCount))
            {
                _active = false;
            }

            // activation takes priority over deactivation
            // bitwise or boolean operations to prevent short-circuiting
            if (_activationLines.MatchesCurrent(lineCount)
                | _activationLinesStdoutOnly.MatchesCurrent(stdoutLineCount)
                | _activationLinesStderrOnly.MatchesCurrent(stderrLineCount)
                | _activationOffsetLines.Remove(lineCount)
                | _activationOffsetStdoutOnlyLines.Remove(stdoutLineCount)
                | _activationOffsetStderrOnlyLines.Remove(stderrLineCount))
            {
                _active = true;
            }

            if (!_active)
            {
                return false;
            }

            if (data != null && EnableExpression != null && EnableExpression.MatchExtractedColor(data) == null)
            {
                return false;
            }

            return true;

            static void MatchExpressions(ICollection<ActivationExpression> expressions, HashSet<long> set, long lineOffset, string data)
            {
                foreach (var expression in expressions)
                {
                    if (expression.Expression.Match(data).Success)
                    {
                        set.Add(lineOffset + expression.ActivationOffset);
                    }
                }
            }
        }

        protected class SortedListIncrementMatch<T> where T : IEquatable<T>
        {
            private readonly List<T> _items;

            private int _index = 0;

            public SortedListIncrementMatch(ICollection<T> items)
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
