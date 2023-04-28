using System;
using System.Collections.Generic;
using System.Text;
using Wilgysef.StdoutHook.Formatters.FormatBuilders;

namespace Wilgysef.StdoutHook.Formatters
{
    internal class FormatFunctionBuilder
    {
        public static FormatBuilder[] FormatBuilders = new[]
        {
            new ColorFormatBuilder(),
        };

        private readonly List<FormatBuilder> _formatBuilders;

        public FormatFunctionBuilder(params FormatBuilder[] formatBuilders)
        {
            _formatBuilders = new List<FormatBuilder>(formatBuilders);
        }

        public FormatFunctionBuilder(IEnumerable<FormatBuilder> formatBuilders)
        {
            _formatBuilders = new List<FormatBuilder>(formatBuilders);
        }

        public static FormatFunctionBuilder Create()
        {
            return new FormatFunctionBuilder(FormatBuilders);
        }

        public Func<string> Build(string key, string format, out bool isConstant)
        {
            foreach (var builder in FormatBuilders)
            {
                if (key.Length == 1 && key[0] == builder.KeyShort
                    || key.Equals(builder.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return builder.Build(format, out isConstant);
                }
            }

            throw new ArgumentException($"Unknown format key: {key}", nameof(key));
        }
    }
}
