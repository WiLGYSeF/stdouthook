using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Profiles
{
    public class Profile
    {
        public string? ProfileName { get; set; }

        public string? Command { get; set; }

        public Regex? CommandExpression { get; set; }

        public string? FullCommandPath { get; set; }

        public Regex? FullCommandPathExpression { get; set; }

        public bool CommandIgnoreCase { get; set; }

        public bool Enabled { get; set; }

        public bool PseudoTty { get; set; }

        public bool Flush { get; set; }

        public IList<object> ArgumentPatterns { get; }

        public int MinArguments { get; set; }

        public int MaxArguments { get; set; }

        public IList<Rule> Rules { get; set; }

        public IDictionary<string, string> ColorAliases { get; set; }

        public IList<string> InheritProfileNames { get; set; }

        public ProfileState? State { get; set; }

        public Profile() { }

        public Profile(ProfileState state)
        {
            State = state;
        }

        public void Build()
        {
            if (State == null)
            {
                throw new Exception($"{nameof(State)} cannot not be null.");
            }

            Build(new Formatter(FormatFunctionBuilder.Create()));
        }

        public bool ApplyRules(ref string line, bool stdout)
        {
            var dataState = new DataState(line, stdout, State!);

            for (var i = 0; i < Rules.Count; i++)
            {
                var rule = Rules[i];
                if (rule.IsActive(dataState))
                {
                    if (rule.Filter)
                    {
                        return false;
                    }

                    try
                    {
                        line = rule.Apply(dataState);
                        dataState.Data = line;

                        if (rule.Terminal)
                        {
                            break;
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
            for (var i = 0; i < Rules.Count; i++)
            {
                Rules[i].Build(State!, formatter);
            }
        }
    }
}
