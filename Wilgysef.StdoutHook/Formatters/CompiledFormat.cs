using System;
using System.Text;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters
{
    internal class CompiledFormat
    {
        public string[] Parts { get; }

        public Func<DataState, string>[] Funcs { get; }

        public CompiledFormat(string[] parts, Func<DataState, string>[] funcs)
        {
            Parts = parts;
            Funcs = funcs;
        }

        public string Compute(DataState state)
        {
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

            builder.Append(Parts[^1]);
            return builder.ToString();
        }
    }
}
