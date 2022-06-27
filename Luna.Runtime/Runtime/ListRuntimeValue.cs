using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Luna.Collections;

namespace Luna.Runtime;

internal class ListRuntimeValue : RuntimeValue, IEnumerable<IRuntimeValue>
{
    private readonly List<IRuntimeValue> _items;

    public int Count => _items.Count;

    public ListRuntimeValue(IEnumerable<IRuntimeValue> items)
    {
        _items = items.ToList();
    }

    public IRuntimeValue GetItem(int index)
    {
        return _items[index];
    }

    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue>? argumentValues = null)
    {
        return new ListRuntimeValue(_items.Select(i => i.GetValue()));
    }

    public override string ToString()
    {
        return $"({String.Join(" ", _items)})";
    }

    public override bool Equals(object? obj)
    {
        return obj is ListRuntimeValue list &&
            _items.Count == list.Count &&
            new HashSet<IRuntimeValue>(_items).IsSubsetOf(list._items);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_items);
    }

    public IEnumerator<IRuntimeValue> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }
}
