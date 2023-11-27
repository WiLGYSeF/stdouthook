using System;
using System.Collections.Generic;
using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Loggers;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Profiles;

public class Profile : IDisposable
{
    private Formatter _formatter = null!;

    public string? ProfileName { get; set; }

    public bool PseudoTty { get; set; }

    public bool Flush { get; set; }

    public int? BufferSize { get; set; }

    public int? OutputFlushInterval { get; set; }

    public bool Interactive { get; set; }

    public int? InteractiveFlushInterval { get; set; }

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

    public string? ApplyRules(string line, bool stdout)
    {
        var dataState = new DataState(line, stdout, this);

        for (var i = 0; i < Rules.Count; i++)
        {
            dataState.Context.Reset();

            var rule = Rules[i];
            if (rule.IsActive(dataState))
            {
                if (rule.Filter)
                {
                    return null;
                }

                try
                {
                    dataState.Data = rule.Apply(dataState);

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

        State.ApplyColorState(dataState.ExtractedColors);

        return dataState.Data;
    }

    public void Split(out Profile stdoutProfile, out Profile stderrProfile)
    {
        stdoutProfile = CopyProperties(true);
        stderrProfile = CopyProperties(false);

        Profile CopyProperties(bool stdout)
        {
            var profile = new Profile
            {
                ProfileName = ProfileName,
                PseudoTty = PseudoTty,
                Flush = Flush,
                BufferSize = BufferSize,
                OutputFlushInterval = OutputFlushInterval,
                Interactive = Interactive,
                InteractiveFlushInterval = InteractiveFlushInterval,
                State = State,
                _formatter = _formatter,
            };

            for (var i = 0; i < Rules.Count; i++)
            {
                var rule = Rules[i];
                if (stdout ? !rule.StderrOnly : !rule.StdoutOnly)
                {
                    profile.Rules.Add(rule.Copy());
                }
            }

            foreach (var (key, val) in CustomColors)
            {
                profile.CustomColors[key] = val;
            }

            return profile;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        State.Dispose();
    }

    internal void Build(Formatter formatter)
    {
        _formatter = formatter;

        for (var i = 0; i < Rules.Count; i++)
        {
            Rules[i].Build(this, formatter);
        }
    }

    internal CompiledFormat CompileFormat(string format)
    {
        return _formatter.CompileFormat(format, this);
    }
}
