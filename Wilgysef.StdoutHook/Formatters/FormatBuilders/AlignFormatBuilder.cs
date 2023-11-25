using System;
using Wilgysef.StdoutHook.Extensions;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal abstract class AlignFormatBuilder : FormatBuilder
    {
        public override Func<FormatComputeState, string> Build(FormatBuildState state, out bool isConstant)
        {
            var format = state.Profile.CompileFormat(GetAlign(state.Contents, out var alignChar, out var alignLength));

            isConstant = format.IsConstant;
            return computeState =>
            {
                var result = format.Compute(computeState.DataState, computeState.Position);
                return result.Length < alignLength
                    ? Align(result, alignChar, alignLength - result.Length)
                    : result;
            };
        }

        protected abstract string Align(string str, char c, int length);

        protected static string GetAlign(string contents, out char alignChar, out int alignLength)
        {
            var contentsSpan = contents.AsSpan();
            var separatorIndex = contentsSpan.IndexOf(Formatter.Separator);
            int alignStartIndex;

            if (separatorIndex > 0 && int.TryParse(contentsSpan[..separatorIndex], out alignLength))
            {
                alignChar = ' ';
                alignStartIndex = separatorIndex + 1;
            }
            else if ((separatorIndex == 1 || (separatorIndex == 0 && contentsSpan[1] == Formatter.Separator))
                && contentsSpan.IndexOfAfter(2, Formatter.Separator) is var index
                && index > 0
                && int.TryParse(contentsSpan[2..index], out alignLength))
            {
                alignChar = contentsSpan[0];
                alignStartIndex = index + 1;
            }
            else
            {
                throw new ArgumentException($"Invalid align format: {contents}");
            }

            return contents[alignStartIndex..];
        }
    }
}
