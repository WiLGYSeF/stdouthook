using System;
using System.Text;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters
{
    internal class CompiledFormat
    {
        public bool IsConstant { get; }

        internal string[] Parts { get; }

        internal Func<FormatComputeState, string>[] Funcs { get; }

        public CompiledFormat(string[] parts, Func<FormatComputeState, string>[] funcs)
        {
            Parts = parts;
            Funcs = funcs;
            IsConstant = Parts.Length == 1;
        }

        public string Compute(DataState dataState)
        {
            return Compute(dataState, 0);
        }

        public string Compute(DataState dataState, int startPosition)
        {
            if (IsConstant)
            {
                return Parts[0];
            }

            var builder = new StringBuilder();
            var computeState = new FormatComputeState(dataState, startPosition);

            for (var i = 0; i < Funcs.Length; i++)
            {
                builder.Append(Parts[i]);

                computeState.SetPosition(builder.Length + startPosition);
                builder.Append(Funcs[i](computeState));
            }

            return builder.Append(Parts[^1])
                .ToString();
        }

        public CompiledFormat Copy()
        {
            return new CompiledFormat(Parts, Funcs);
        }
    }
}
