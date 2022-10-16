using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Luna.Collections;

public class ReadonlyArray<T> : IEnumerable<T>, IEquatable<ReadonlyArray<T>?>
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

    public override bool Equals(object? obj)
    {
        return Equals(obj as ReadonlyArray<T>);
    }

    public bool Equals(ReadonlyArray<T>? other)
    {
        return other is not null &&
               _items.SequenceEqual(other._items);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var item in _items)
        {
            hashCode.Add(item?.GetHashCode());
        }

        return hashCode.ToHashCode();
    }
}

public static class ReadonlyArrayEnumerableExt
{
    public static ReadonlyArray<T> ToReadonlyArray<T>(this IEnumerable<T> collection) => new(collection);
}
