using System;
using System.Collections.Generic;
using Wilgysef.StdoutHook.Formatters.FormatBuilders;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters
{
    internal class FormatFunctionBuilder
    {
        public static FormatBuilder[] FormatBuilders = new FormatBuilder[]
        {
            new ByteFormatBuilder(),
            new ColorFormatBuilder(),
            new DataFormatBuilder(),
            new FieldFormatBuilder(),
            new ProcessFormatBuilder(),
            new ProfileFormatBuilder(),
            new RegexGroupFormatBuilder(),
            new TimeFormatBuilder(),
        };

        private readonly List<FormatBuilder> _formatBuilders;

        public static FormatFunctionBuilder Create()
        {
            return new FormatFunctionBuilder(FormatBuilders);
        }

        public FormatFunctionBuilder(params FormatBuilder[] formatBuilders)
        {
            _formatBuilders = new List<FormatBuilder>(formatBuilders);
        }

        public FormatFunctionBuilder(IEnumerable<FormatBuilder> formatBuilders)
        {
            _formatBuilders = new List<FormatBuilder>(formatBuilders);
        }

        public void SetCustomColors(IDictionary<string, string> colors)
        {
            foreach (var formatBuilder in _formatBuilders)
            {
                if (formatBuilder is ColorFormatBuilder colorFormatBuilder)
                {
                    colorFormatBuilder.CustomColors.Clear();
                    foreach (var (key, val) in colors)
                    {
                        colorFormatBuilder.CustomColors[key] = val;
                    }
                }
            }
        }

        public Func<DataState, string> Build(
            string key,
            string contents,
            Profile profile,
            out bool isConstant)
        {
            foreach (var builder in _formatBuilders)
            {
                if (builder.Key != null && key.Equals(builder.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return builder.Build(new FormatBuildState(contents, profile), out isConstant);
                }

                if (builder.KeyShort.HasValue && key.Length >= 1 && key[0] == builder.KeyShort.Value)
                {
                    return builder.Build(new FormatBuildState(key[1..] + contents, profile), out isConstant);
                }
            }

            throw new ArgumentException($"Unknown format key: {key}", nameof(key));
        }
    }
}
