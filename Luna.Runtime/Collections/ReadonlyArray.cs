using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Luna.Collections;

public class ReadonlyArray<T> : IEnumerable<T>
{
    private readonly T[] _items;

    public ReadonlyArray(IEnumerable<T> items)
    {
        _items = items.ToArray();
    }

    public T this[int index] => _items[index];

    public int Count => _items.Length;

    public IEnumerator<T> GetEnumerator()
    {
        return _items.ToList().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }
}
