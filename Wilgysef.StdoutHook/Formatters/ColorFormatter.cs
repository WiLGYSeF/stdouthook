﻿using System.Collections.Generic;
using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters;

public class ColorFormatter
{
    public IDictionary<string, string> CustomColors { get; set; } = new Dictionary<string, string>();

    public bool InvalidFormatBlank { get; set; }

    public string Format(string format)
    {
        var colorFormatBuilder = new ColorFormatBuilder();

        foreach (var (key, val) in CustomColors)
        {
            colorFormatBuilder.CustomColors.Add(key, val);
        }

        var formatter = new Formatter(new FormatFunctionBuilder(colorFormatBuilder))
        {
            InvalidFormatBlank = InvalidFormatBlank,
        };

        using var profile = new Profile();
        return formatter.Format(format, new DataState(profile));
    }
}
