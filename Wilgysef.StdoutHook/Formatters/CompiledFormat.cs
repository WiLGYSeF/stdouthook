using System;
using System.Text;

namespace Wilgysef.StdoutHook.Formatters
{
    internal class CompiledFormat
    {
        public string[] Parts { get; }

        public Func<string>[] Funcs { get; }

        public CompiledFormat(string[] parts, Func<string>[] funcs)
        {
            Parts = parts;
            Funcs = funcs;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            for (var i = 0; i < Funcs.Length; i++)
            {
                var part = Parts[i];
                if (part.Length > 0)
                {
                    builder.Append(part);
                }

                builder.Append(Funcs[i]());
            }

            builder.Append(Parts[^1]);
            return builder.ToString();
        }
    }
}
