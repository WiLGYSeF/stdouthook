using System;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class RegexGroupFormatBuilder : FormatBuilder
    {
        public override string? Key => "group";

        public override char? KeyShort => 'G';

        public override Func<DataState, string> Build(FormatBuildState state, out bool isConstant)
        {
            if (state.Contents.Length == 0)
            {
                throw new ArgumentException("Group must be specified.");
            }

            var contents = state.Contents;
            isConstant = false;

            if (contents.Equals("c", StringComparison.OrdinalIgnoreCase))
            {
                return dataState =>
                {
                    var context = dataState.Context.RegexGroupContext;
                    if (context == null)
                    {
                        return "";
                    }

                    var groupNumber = context.GetCurrentGroupNumber();

                    return groupNumber <= context.Groups!.Count
                        ? context.Groups[groupNumber.ToString()]
                        : "";
                };
            }

            return dataState =>
            {
                var context = dataState.Context.RegexGroupContext;
                if (context == null)
                {
                    return "";
                }

                return context.Groups.TryGetValue(contents, out var value)
                    ? value
                    : "";
            };
        }
    }
}
