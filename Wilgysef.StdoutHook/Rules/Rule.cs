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

        internal abstract string Apply(DataState state);

        protected abstract Rule CopyInternal();

        internal virtual void Build(Profile profile, Formatter formatter)
        {
            if (StdoutOnly && StderrOnly)
            {
                throw new InvalidOperationException("Rule cannot be both stdout only and stderr only");
            }

            Formatter = formatter;

            _activationLines = new SortedListIncrementMatch<long>(ActivationLines);
            _activationLinesStdoutOnly = new SortedListIncrementMatch<long>(ActivationLinesStdoutOnly);
            _activationLinesStderrOnly = new SortedListIncrementMatch<long>(ActivationLinesStderrOnly);
            _deactivationLines = new SortedListIncrementMatch<long>(DeactivationLines);
            _deactivationLinesStdoutOnly = new SortedListIncrementMatch<long>(DeactivationLinesStdoutOnly);
            _deactivationLinesStderrOnly = new SortedListIncrementMatch<long>(DeactivationLinesStderrOnly);

            if (ActivationLines.Count > 0
                || ActivationLinesStdoutOnly.Count > 0
                || ActivationLinesStderrOnly.Count > 0)
            {
                var activationLineMins = new long[]
                {
                    _activationLines.GetFirstOrDefault(long.MaxValue),
                    _activationLinesStdoutOnly.GetFirstOrDefault(long.MaxValue),
                    _activationLinesStderrOnly.GetFirstOrDefault(long.MaxValue),
                };

                var deactivationLineMins = new long[]
                {
                    _deactivationLines.GetFirstOrDefault(long.MaxValue),
                    _deactivationLinesStdoutOnly.GetFirstOrDefault(long.MaxValue),
                    _deactivationLinesStderrOnly.GetFirstOrDefault(long.MaxValue),
                };

                _active = !DoesFirstCollectionContainMin(activationLineMins, deactivationLineMins);
            }
            else if (ActivationExpressions.Count + ActivationExpressionsStdoutOnly.Count + ActivationExpressionsStderrOnly.Count > 0
                && DeactivationExpressions.Count + DeactivationExpressionsStdoutOnly.Count + DeactivationExpressionsStderrOnly.Count == 0)
            {
                _active = false;
            }
        }

        internal virtual bool IsActive(DataState state)
        {
            if (StdoutOnly && !state.Stdout
                || StderrOnly && state.Stdout)
            {
                return false;
            }

            var profileState = state.Profile.State;

            // avoid potential race conditions by caching values
            var lineCount = profileState.LineCount;
            var stdoutLineCount = profileState.StdoutLineCount;
            var stderrLineCount = profileState.StderrLineCount;

            var data = state.DataTrimEndNewline;

            if (data != null)
            {
                MatchExpressions(ActivationExpressions, _activationOffsetLines, lineCount, data);
                MatchExpressions(DeactivationExpressions, _deactivationOffsetLines, lineCount, data);

                if (!StderrOnly)
                {
                    MatchExpressions(ActivationExpressionsStdoutOnly, _activationOffsetStdoutOnlyLines, stdoutLineCount, data);
                    MatchExpressions(DeactivationExpressionsStdoutOnly, _deactivationOffsetStdoutOnlyLines, stdoutLineCount, data);
                }

                if (!StdoutOnly)
                {
                    MatchExpressions(ActivationExpressionsStderrOnly, _activationOffsetStderrOnlyLines, stderrLineCount, data);
                    MatchExpressions(DeactivationExpressionsStderrOnly, _deactivationOffsetStderrOnlyLines, stderrLineCount, data);
                }
            }

            // bitwise-or boolean operations to prevent short-circuiting
            var active = _activationLines.MatchesCurrent(lineCount) | _activationOffsetLines.Remove(lineCount);
            var deactivate = _deactivationLines.MatchesCurrent(lineCount) | _deactivationOffsetLines.Remove(lineCount);

            if (!StderrOnly)
            {
                active |= _activationLinesStdoutOnly.MatchesCurrent(stdoutLineCount) | _activationOffsetStdoutOnlyLines.Remove(stdoutLineCount);
                deactivate |= _deactivationLinesStdoutOnly.MatchesCurrent(stdoutLineCount) | _deactivationOffsetStdoutOnlyLines.Remove(stdoutLineCount);
            }

            if (!StdoutOnly)
            {
                active |= _activationLinesStderrOnly.MatchesCurrent(stderrLineCount) | _activationOffsetStderrOnlyLines.Remove(stderrLineCount);
                deactivate |= _deactivationLinesStderrOnly.MatchesCurrent(stderrLineCount) | _deactivationOffsetStderrOnlyLines.Remove(stderrLineCount);
            }

            if (deactivate)
            {
                _active = false;
            }

            // activation takes priority over deactivation
            if (active)
            {
                _active = true;
            }

            if (!_active
                || (data != null
                    && EnableExpression != null
                    && !EnableExpression.IsMatch(state.DataExtractedColorTrimEndNewline)))
            {
                return false;
            }

            return true;

            static void MatchExpressions(ICollection<ActivationExpression> expressions, HashSet<long> set, long lineOffset, string data)
            {
                // optimized for empty collections
                if (expressions.Count > 0)
                {
                    foreach (var expression in expressions)
                    {
                        if (expression.Expression.IsMatch(data))
                        {
                            set.Add(lineOffset + expression.ActivationOffset);
                        }
                    }
                }
            }
        }

        internal Rule Copy()
        {
            var rule = CopyInternal();

            rule.EnableExpression = EnableExpression;
            rule.StdoutOnly = StdoutOnly;
            rule.StderrOnly = StderrOnly;
            rule.Terminal = Terminal;
            rule.TrimNewline = TrimNewline;
            rule.ActivationLines.AddRange(ActivationLines);
            rule.DeactivationLines.AddRange(DeactivationLines);
            rule.ActivationExpressions.AddRange(ActivationExpressions);
            rule.DeactivationExpressions.AddRange(DeactivationExpressions);
            rule.Filter = Filter;
            rule.Formatter = Formatter;

            rule._activationLines = new SortedListIncrementMatch<long>(ActivationLines);
            rule._deactivationLines = new SortedListIncrementMatch<long>(DeactivationLines);
            rule._activationOffsetLines = new HashSet<long>();
            rule._deactivationOffsetLines = new HashSet<long>();
            rule._active = _active;

            if (!StderrOnly)
            {
                rule.ActivationLinesStdoutOnly.AddRange(ActivationLinesStdoutOnly);
                rule.DeactivationLinesStdoutOnly.AddRange(DeactivationLinesStdoutOnly);

                rule.ActivationExpressionsStdoutOnly.AddRange(ActivationExpressionsStdoutOnly);
                rule.DeactivationExpressionsStdoutOnly.AddRange(DeactivationExpressionsStdoutOnly);

                rule._activationLinesStdoutOnly = new SortedListIncrementMatch<long>(ActivationLinesStdoutOnly);
                rule._deactivationLinesStdoutOnly = new SortedListIncrementMatch<long>(DeactivationLinesStdoutOnly);

                rule._activationOffsetStdoutOnlyLines = new HashSet<long>();
                rule._deactivationOffsetStdoutOnlyLines = new HashSet<long>();
            }

            if (!StdoutOnly)
            {
                rule.ActivationLinesStderrOnly.AddRange(ActivationLinesStderrOnly);
                rule.DeactivationLinesStderrOnly.AddRange(DeactivationLinesStderrOnly);

                rule.ActivationExpressionsStderrOnly.AddRange(ActivationExpressionsStderrOnly);
                rule.DeactivationExpressionsStderrOnly.AddRange(DeactivationExpressionsStderrOnly);

                rule._activationLinesStderrOnly = new SortedListIncrementMatch<long>(ActivationLinesStderrOnly);
                rule._deactivationLinesStderrOnly = new SortedListIncrementMatch<long>(DeactivationLinesStderrOnly);

                rule._activationOffsetStderrOnlyLines = new HashSet<long>();
                rule._deactivationOffsetStderrOnlyLines = new HashSet<long>();
            }

            return rule;
        }

        protected class SortedListIncrementMatch<T> where T : IEquatable<T>
        {
            private readonly List<T> _items;
            private readonly int _itemCount;

            private int _index = 0;

            public SortedListIncrementMatch(ICollection<T> items)
            {
                _items = new List<T>(items);
                _items.Sort();

                _itemCount = _items.Count;
            }

            public bool MatchesCurrent(T item)
            {
                if (_index == _itemCount)
                {
                    return false;
                }

                var originalIndex = _index;

                for (; _index < _itemCount && item.Equals(_items[_index]); _index++) ;

                return originalIndex != _index;
            }

            public T GetFirstOrDefault(T defaultValue)
            {
                return _itemCount > 0
                    ? _items[0]
                    : defaultValue;
            }
        }

        private static bool DoesFirstCollectionContainMin(long[] first, long[] second)
        {
            var min = long.MaxValue;

            foreach (var val in first)
            {
                if (val < min)
                {
                    min = val;
                }
            }

            var secondMatch = false;

            foreach (var val in second)
            {
                if (val < min)
                {
                    return false;
                }
                else if (val == min)
                {
                    secondMatch = true;
                }
            }

            return !secondMatch;
        }
    }
}
