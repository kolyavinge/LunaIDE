using System.Collections.Generic;

namespace Luna.Utils;

public static class SetExt
{
    public static void AddRange<T>(this ISet<T> set, IEnumerable<T> collection)
    {
        foreach (var item in collection)
        {
            set.Add(item);
        }
    }
}
