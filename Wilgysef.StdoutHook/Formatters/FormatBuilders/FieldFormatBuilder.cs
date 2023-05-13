using System;
using System.Text;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class FieldFormatBuilder : FormatBuilder
    {
        public override string? Key => "field";

        public override char? KeyShort => 'F';

        public override Func<DataState, string> Build(FormatBuildState state, out bool isConstant)
        {
            if (state.Contents.Length == 0)
            {
                throw new ArgumentException("Field must be specified.");
            }

            var contents = state.Contents;
            isConstant = false;

            if (contents.Equals("c", StringComparison.OrdinalIgnoreCase))
            {
                return dataState =>
                {
                    var context = dataState.Context.FieldContext;
                    if (context == null)
                    {
                        return "";
                    }

                    var fieldNumber = context.GetCurrentFieldNumber();

                    return fieldNumber <= context.Fields.Count
                        ? context.Fields[fieldNumber - 1]
                        : "";
                };
            }

            if (contents == "#")
            {
                return dataState =>
                {
                    var context = dataState.Context.FieldContext;
                    if (context == null)
                    {
                        return "";
                    }

                    return context.Fields.Count.ToString();
                };
            }

            var separator = false;

            if (contents[0] == 'S' || contents[0] == 's')
            {
                contents = contents[1..];
                separator = true;
            }

            if (!FieldRange.TryParse(contents, out var fieldRange))
            {
                throw new ArgumentException($"Invalid field: {contents}.");
            }

            if (separator && !fieldRange.SingleValue.HasValue)
            {
                throw new ArgumentException("Field separator must be a single number");
            }

            if (fieldRange.SingleValue.HasValue)
            {
                if (separator)
                {
                    return dataState =>
                    {
                        var context = dataState.Context.FieldContext;
                        if (context == null)
                        {
                            return "";
                        }

                        var singleVal = fieldRange.SingleValue.Value;

                        return singleVal <= context.FieldSeparators.Count
                            ? context.FieldSeparators[singleVal - 1]
                            : "";
                    };
                }
                else
                {
                    return dataState =>
                    {
                        var context = dataState.Context.FieldContext;
                        if (context == null)
                        {
                            return "";
                        }

                        var singleVal = fieldRange.SingleValue.Value;

                        return singleVal <= context.Fields.Count
                            ? context.Fields[singleVal - 1]
                            : "";
                    };
                }
            }

            return dataState =>
            {
                var context = dataState.Context.FieldContext;
                if (context == null)
                {
                    return "";
                }

                var builder = new StringBuilder();

                for (var i = fieldRange.Min; i < context.Fields.Count && i <= fieldRange.Max; i++)
                {
                    builder.Append(context.Fields[i - 1]);
                    if (i < fieldRange.Max)
                    {
                        builder.Append(context.FieldSeparators[i - 1]);
                    }
                }

                return builder.ToString();
            };
        }
    }
}
