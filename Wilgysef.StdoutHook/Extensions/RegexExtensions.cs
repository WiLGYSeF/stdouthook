using System.Text.RegularExpressions;

namespace Wilgysef.StdoutHook.Extensions
{
    internal static class RegexExtensions
    {
        public static string[] SplitWithSeparators(this Regex regex, string input, out int count)
        {
            var matchCollection = regex.Matches(input);
            var results = new string[matchCollection.Count * 2 + 1];
            var resultIndex = 0;
            var lastIndex = 0;

            for (var i = 0; i < matchCollection.Count; i++)
            {
                var match = matchCollection[i];
                results[resultIndex++] = input[lastIndex..match.Index];
                results[resultIndex++] = match.Value;
                lastIndex = match.Index + match.Value.Length;
            }

            results[resultIndex] = input[lastIndex..];
            count = results.Length / 2 + 1;
            return results;
        }
    }
}
