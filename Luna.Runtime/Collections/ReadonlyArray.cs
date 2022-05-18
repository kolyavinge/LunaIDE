using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Luna.Collections;

public class ReadonlyArray<T> : IEnumerable<T>
{
    private readonly List<T> _items;

    public ReadonlyArray(IEnumerable<T> items)
    {
        _items = items.ToList();
    }

    public ReadonlyArray()
    {
        _items = new List<T>();
    }

    public T this[int index] => _items[index];

    public int Count => _items.Count;

    public IEnumerator<T> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }
}

public static class ReadonlyArrayEnumerableExt
{
    public static ReadonlyArray<T> ToReadonlyArray<T>(this IEnumerable<T> collection) => new(collection);
}
