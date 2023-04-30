using System;
using System.Collections.Generic;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Profiles
{
    public class Profile
    {
        public ProfileState State { get; }

        public IList<Rule> Rules => _rules;

        private readonly List<Rule> _rules = new List<Rule>();

        public Profile(ProfileState state)
        {
            State = state;
        }

        public void Build()
        {
            Build(new Formatter(FormatFunctionBuilder.Create()));
        }

        public bool ApplyRules(ref string line, bool stdout)
        {
            var dataState = new DataState(line, stdout, State);

            for (var i = 0; i < _rules.Count; i++)
            {
                var rule = _rules[i];
                if (rule.IsActive(dataState))
                {
                    try
                    {
                        line = rule.Apply(dataState);
                        dataState.Data = line;

                        if (rule.Terminal)
                        {
                            return false;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            return true;
        }

        internal void Build(Formatter formatter)
        {
            for (var i = 0; i < _rules.Count; i++)
            {
                _rules[i].Build(State, formatter);
            }
        }
    }
}
