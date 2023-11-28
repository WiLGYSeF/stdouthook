using System;

namespace Wilgysef.StdoutHook.Extensions;

/// <summary>
/// <see cref="ReadOnlySpan{T}"/> extensions.
/// </summary>
internal static class ReadOnlySpanExtensions
{
    /// <summary>
    /// Gets the index of <paramref name="item"/> after the <paramref name="startIndex"/> start index.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="span">Span.</param>
    /// <param name="startIndex">Start index.</param>
    /// <param name="item">Item.</param>
    /// <returns>Index of item, or <c>-1</c> if it is not found.</returns>
    public static int IndexOfAfter<T>(this ReadOnlySpan<T> span, int startIndex, T item)
        where T : IEquatable<T>
    {
        var index = span[startIndex..].IndexOf(item);
        return index == -1
            ? -1
            : index + startIndex;
    }
}
