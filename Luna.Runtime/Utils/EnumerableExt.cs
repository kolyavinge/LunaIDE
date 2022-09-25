﻿using System.Collections.Generic;

namespace Luna.Utils;

public static class EnumerableExt
{
    public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (var item in collection)
        {
            action(item);
        }
    }

    public static void Each<T>(this IEnumerable<T> collection, Action<T, int> action)
    {
        int i = 0;
        foreach (var item in collection)
        {
            action(item, i);
            i++;
        }
    }

    public static (IEnumerable<T>, IEnumerable<T>) Map<T>(this IEnumerable<T> collection, Predicate<T> predicate)
    {
        var resultTrue = new List<T>();
        var resultFalse = new List<T>();

        foreach (var item in collection)
        {
            if (predicate(item))
            {
                resultTrue.Add(item);
            }
            else
            {
                resultFalse.Add(item);
            }
        }

        return (resultTrue, resultFalse);
    }
}
