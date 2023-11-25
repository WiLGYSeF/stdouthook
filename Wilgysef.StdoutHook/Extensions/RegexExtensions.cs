using System.Collections.Generic;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Extensions;

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

    public static List<MatchGroup[]> MatchAllGroups(this Regex regex, string input)
    {
        var matches = regex.Matches(input);
        var groupList = new List<MatchGroup[]>();

        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var groups = new MatchGroup[match.Groups.Count];

            for (var groupIndex = 0; groupIndex < match.Groups.Count; groupIndex++)
            {
                var curMatch = match.Groups[groupIndex];
                groups[groupIndex] = new MatchGroup(curMatch.Value, curMatch.Name, curMatch.Index);
            }

            groupList.Add(groups);
        }

        return groupList;
    }
}
