using System;
using System.Text;

namespace Wilgysef.StdoutHook.Formatters
{
    internal class CompiledFormat
    {
        private readonly string[] _parts;

        private readonly Func<string>[] _funcs;

        public CompiledFormat(string[] parts, Func<string>[] funcs)
        {
            _parts = parts;
            _funcs = funcs;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            for (var i = 0; i < _funcs.Length; i++)
            {
                var part = _parts[i];
                if (part.Length > 0)
                {
                    builder.Append(part);
                }

                builder.Append(_funcs[i]());
            }

            builder.Append(_parts[^1]);
            return builder.ToString();
        }
    }
}
