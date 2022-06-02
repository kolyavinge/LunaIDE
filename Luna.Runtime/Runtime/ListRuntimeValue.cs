﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Luna.Runtime;

internal class ListRuntimeValue : IRuntimeValue
{
    private readonly List<IRuntimeValue> _runtimeValues;

    public int Count => _runtimeValues.Count;

    public ListRuntimeValue(IEnumerable<IRuntimeValue> items)
    {
        _runtimeValues = items.ToList();
    }

    public IRuntimeValue GetItem(int index)
    {
        return _runtimeValues[index];
    }

    public override string ToString()
    {
        return String.Format("({0})", String.Join(" ", _runtimeValues));
    }

    public override bool Equals(object? obj)
    {
        return obj is ListRuntimeValue list &&
            _runtimeValues.Count == list.Count &&
            new HashSet<IRuntimeValue>(_runtimeValues).IsSubsetOf(list._runtimeValues);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_runtimeValues);
    }
}