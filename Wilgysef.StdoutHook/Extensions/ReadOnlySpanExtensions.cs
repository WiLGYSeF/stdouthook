using System;

namespace Wilgysef.StdoutHook.Extensions;

internal static class ReadOnlySpanExtensions
{
    public static int IndexOfAfter<T>(this ReadOnlySpan<T> span, int startIndex, T item)
        where T : IEquatable<T>
    {
        var index = span[startIndex..].IndexOf(item);
        return index == -1
            ? -1
            : index + startIndex;
    }
}
