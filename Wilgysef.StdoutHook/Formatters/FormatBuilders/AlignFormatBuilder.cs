using System;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class AlignFormatBuilder : FormatBuilder
    {
        public override string? Key => "align";

        public override char? KeyShort => null;

        public override Func<DataState, string> Build(FormatBuildState state, out bool isConstant)
        {
            var contentsSpan = state.Contents.AsSpan();
            var separatorIndex = contentsSpan.IndexOf(Formatter.Separator);
            char alignChar;
            int alignStartIndex;

            if (separatorIndex > 0 && int.TryParse(contentsSpan[..separatorIndex], out var alignLength))
            {
                alignChar = ' ';
                alignStartIndex = separatorIndex + 1;
            }
            else if ((separatorIndex == 1 || separatorIndex == 0 && contentsSpan[1] == Formatter.Separator)
                && contentsSpan[2..].IndexOf(Formatter.Separator) is var specifiedCharIndex
                && specifiedCharIndex > 0
                && int.TryParse(contentsSpan[2..(specifiedCharIndex + 2)], out alignLength))
            {
                alignChar = contentsSpan[0];
                alignStartIndex = specifiedCharIndex + 3;
            }
            else if (separatorIndex == 0
                && contentsSpan[1..].IndexOf(Formatter.Separator) is var implicitCharIndex
                && implicitCharIndex > 0
                && int.TryParse(contentsSpan[1..(implicitCharIndex + 1)], out alignLength))
            {
                alignChar = Formatter.Separator;
                alignStartIndex = implicitCharIndex + 2;
            }
            else
            {
                throw new ArgumentException($"Invalid align format: {state.Contents}");
            }

            var format = state.Profile.Formatter.CompileFormat(
                contentsSpan[alignStartIndex..].ToString(),
                state.Profile);

            isConstant = format.IsConstant;
            return dataState =>
            {
                var result = format.Compute(dataState);
                return result.Length < alignLength
                    ? new string(alignChar, alignLength - result.Length) + result
                    : result;
            };
        }
    }
}
