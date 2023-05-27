using System;
using System.Text;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters
{
    internal class CompiledFormat
    {
        public bool IsConstant { get; }

        internal string[] Parts { get; }

        internal Func<DataState, string>[] Funcs { get; }

        public CompiledFormat(string[] parts, Func<DataState, string>[] funcs)
        {
            Parts = parts;
            Funcs = funcs;
            IsConstant = Parts.Length == 1;
        }

        public string Compute(DataState state)
        {
            if (IsConstant)
            {
                return Parts[0];
            }

            var builder = new StringBuilder();

            for (var i = 0; i < Funcs.Length; i++)
            {
                var part = Parts[i];
                if (part.Length > 0)
                {
                    builder.Append(part);
                }

                builder.Append(Funcs[i](state));
            }

            return builder.Append(Parts[^1])
                .ToString();
        }
    }
}
