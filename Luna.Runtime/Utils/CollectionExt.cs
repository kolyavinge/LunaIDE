using System.Collections.Generic;

namespace Luna.Utils;

public static class CollectionExt
{
    public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Remove(item);
        }
    }
}
