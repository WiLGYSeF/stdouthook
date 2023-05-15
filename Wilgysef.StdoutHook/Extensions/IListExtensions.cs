using System;
using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Extensions
{
    internal static class IListExtensions
    {
        public static void InsertRange<T>(this IList<T> list, int index, IList<T> items)
        {
            var startListCount = list.Count;

            if (index < 0 || index > startListCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var itemsStartIndex = startListCount - index;

            if (itemsStartIndex >= 0)
            {
                for (var i = itemsStartIndex; i < items.Count; i++)
                {
                    list.Add(items[i]);
                }
            }

            for (var i = startListCount > items.Count ? startListCount - items.Count : index; i < startListCount; i++)
            {
                list.Add(list[i]);
            }

            var limit = Math.Max(index, items.Count);
            for (var i = startListCount - 1; i >= limit; i--)
            {
                list[i] = list[i - items.Count];
            }

            limit = Math.Min(itemsStartIndex, items.Count);
            for (var i = 0; i < limit; i++)
            {
                list[i + index] = items[i];
            }
        }
    }
}
