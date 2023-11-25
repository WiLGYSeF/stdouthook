using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Extensions;

/// <summary>
/// <see cref="ICollection"/> extensions.
/// </summary>
internal static class ICollectionExtensions
{
    /// <summary>
    /// Adds items of <paramref name="items"/> to <paramref name="collection"/>.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="collection">Collection.</param>
    /// <param name="items">Items.</param>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}
