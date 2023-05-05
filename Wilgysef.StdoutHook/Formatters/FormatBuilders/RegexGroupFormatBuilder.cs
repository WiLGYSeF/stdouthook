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

            if (!int.TryParse(contents, out var groupNumber))
            {
                throw new ArgumentException($"Invalid group: {contents}.");
            }

            isConstant = false;

            return dataState =>
            {
                if (dataState.Context.RegexGroupContext == null)
                {
                    return "";
                }

                var context = dataState.Context.RegexGroupContext;

                return groupNumber <= context.Groups!.Count
                    ? context.Groups[groupNumber]
                    : "";
            };
        }
    }
}
