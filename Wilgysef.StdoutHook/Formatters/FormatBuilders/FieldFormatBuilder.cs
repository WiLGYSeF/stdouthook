using System;
using System.Text;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class FieldFormatBuilder : FormatBuilder
    {
        public override string? Key => "field";

        public override char? KeyShort => 'F';

        public override Func<FormatComputeState, string> Build(FormatBuildState state, out bool isConstant)
        {
            if (state.Contents.Length == 0)
            {
                throw new ArgumentException("Field must be specified.");
            }

            var contents = state.Contents;
            isConstant = false;

            if (contents.Equals("c", StringComparison.OrdinalIgnoreCase))
            {
                return GetCurrentField;
            }

            if (contents == "#")
            {
                return GetFieldCount;
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
                var singleValue = fieldRange.SingleValue.Value;

                if (separator)
                {
                    return computeState =>
                    {
                        var context = computeState.DataState.Context.FieldContext;
                        if (context == null)
                        {
                            return "";
                        }

                        return singleValue <= context.FieldSeparators.Count
                            ? context.FieldSeparators[singleValue - 1]
                            : "";
                    };
                }
                else
                {
                    return computeState =>
                    {
                        var context = computeState.DataState.Context.FieldContext;
                        if (context == null)
                        {
                            return "";
                        }

                        return singleValue <= context.Fields.Count
                            ? context.Fields[singleValue - 1]
                            : "";
                    };
                }
            }

            return computeState =>
            {
                var context = computeState.DataState.Context.FieldContext;
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

            static string GetCurrentField(FormatComputeState computeState)
            {
                var context = computeState.DataState.Context.FieldContext;
                if (context == null)
                {
                    return "";
                }

                var fieldNumber = context.GetCurrentFieldNumber();

                return fieldNumber <= context.Fields.Count
                    ? context.Fields[fieldNumber - 1]
                    : "";
            }

            static string GetFieldCount(FormatComputeState computeState)
            {
                var context = computeState.DataState.Context.FieldContext;
                return context?.Fields.Count.ToString() ?? "";
            }
        }
    }
}
