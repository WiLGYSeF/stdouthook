using System;
using System.Collections.Generic;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Loggers;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Profiles
{
    public class Profile : IDisposable
    {
        public string? ProfileName { get; set; }

        public bool PseudoTty { get; set; }

        public bool Flush { get; set; }

        public IList<Rule> Rules { get; set; } = new List<Rule>();

        public IDictionary<string, string> CustomColors { get; set; } = new Dictionary<string, string>();

        public ProfileState State { get; set; } = new ProfileState();

        public void Build()
        {
            var formatFunctionBuilder = FormatFunctionBuilder.Create();

            if (CustomColors.Count > 0)
            {
                formatFunctionBuilder.SetCustomColors(CustomColors);
            }

            Build(new Formatter(formatFunctionBuilder));
        }

        public bool ApplyRules(ref string line, bool stdout)
        {
            var dataState = new DataState(line, stdout, this);

            for (var i = 0; i < Rules.Count; i++)
            {
                dataState.ResetContext();

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
                    catch (Exception ex)
                    {
                        GlobalLogger.Error(ex, "exception occurred when applying rule");
                    }
                }
            }

            return true;
        }

        public void Dispose()
        {
            State.Dispose();
        }

        internal void Build(Formatter formatter)
        {
            for (var i = 0; i < Rules.Count; i++)
            {
                Rules[i].Build(this, formatter);
            }
        }
    }
}
