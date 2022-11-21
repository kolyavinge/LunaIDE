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
        if (obj is not ListRuntimeValue list) return false;
        var result = _items.Count == list.Count;
        if (!result) return false;
        for (int i = 0; i < Count; i++)
        {
            if (!_items[i].Equals(list._items[i])) return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var item in _items)
        {
            hashCode.Add(item.GetHashCode());
        }

        return hashCode.ToHashCode();
    }

    public IEnumerator<IRuntimeValue> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
}
