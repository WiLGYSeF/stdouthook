using System;
using System.Collections.Generic;
using System.Text;

namespace Wilgysef.StdoutHook.Utilities
{
    internal static class StringExpressions
    {
        public static string? GetExpression(object? obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is string str)
            {
                return str;
            }

            if (obj is IList<object?> list)
            {
                var builder = new StringBuilder();

                for (var i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is string item))
                    {
                        throw new Exception();
                    }

                    builder.Append(item);
                }

                return builder.ToString();
            }

            throw new Exception();
        }
    }
}
