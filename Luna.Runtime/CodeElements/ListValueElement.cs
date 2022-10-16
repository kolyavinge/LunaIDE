using System.Collections.Generic;
using Luna.Collections;

namespace Luna.CodeElements;

public class ListValueElement : ValueElement, IEquatable<ListValueElement?>
{
    public ReadonlyArray<ValueElement> Items { get; }

    public ListValueElement(IEnumerable<ValueElement> items, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Items = new ReadonlyArray<ValueElement>(items);
    }

    internal ListValueElement(IEnumerable<ValueElement> items) : this(items, 0, 0) { }

    public override string ToString()
    {
        return $"({String.Join(" ", Items)})";
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ListValueElement);
    }

    public bool Equals(ListValueElement? other)
    {
        return other is not null &&
               base.Equals(other) &&
               Items.Equals(other.Items);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Items);
    }
}
