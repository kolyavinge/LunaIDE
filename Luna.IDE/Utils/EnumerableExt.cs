using System;
using System.Collections.Generic;
using System.Text;

namespace Luna.IDE.Utils
{
    internal static class EnumerableExt
    {
        public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }
}
