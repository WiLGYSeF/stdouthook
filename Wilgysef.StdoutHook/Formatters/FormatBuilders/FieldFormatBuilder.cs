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

            isConstant = false;

            if (fieldRange.SingleValue.HasValue)
            {
                if (separator)
                {
                    return dataState =>
                    {
                        if (!Preface(dataState, fieldRange))
                        {
                            return "";
                        }

                        var singleVal = fieldRange.SingleValue.Value;
                        return singleVal <= dataState.Context.FieldSeparators!.Count
                            ? dataState.Context.FieldSeparators[singleVal - 1]
                            : "";
                    };
                }
                else
                {
                    return dataState =>
                    {
                        if (!Preface(dataState, fieldRange))
                        {
                            return "";
                        }

                        var singleVal = fieldRange.SingleValue.Value;
                        return singleVal <= dataState.Context.Fields!.Count
                            ? dataState.Context.Fields[singleVal - 1]
                            : "";
                    };
                }
            }

            return dataState =>
            {
                if (!Preface(dataState, fieldRange))
                {
                    return "";
                }

                var builder = new StringBuilder();

                for (var i = fieldRange.Min; i < dataState.Context.Fields!.Count && i <= fieldRange.Max; i++)
                {
                    builder.Append(dataState.Context.Fields[i - 1]);
                    if (i < fieldRange.Max)
                    {
                        builder.Append(dataState.Context.FieldSeparators![i - 1]);
                    }
                }

                return builder.ToString();
            };
        }

        private static bool Preface(DataState dataState, FieldRange range)
        {
            if (!dataState.Context.HasFields)
            {
                return false;
            }

            if (range.Max.HasValue && range.Max.Value > dataState.Context.HighestFieldNumber)
            {
                dataState.Context.HighestFieldNumber = range.Max.Value;
            }

            return true;
        }
    }
}
