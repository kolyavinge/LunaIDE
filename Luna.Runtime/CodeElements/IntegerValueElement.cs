namespace Luna.CodeElements;

public class IntegerValueElement : ValueElement, IEquatable<IntegerValueElement?>
{
    public long Value { get; }

    public IntegerValueElement(long value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Value = value;
    }

    internal IntegerValueElement(long value) : this(value, 0, 0) { }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as IntegerValueElement);
    }

    public bool Equals(IntegerValueElement? other)
    {
        return other is not null &&
               base.Equals(other) &&
               Value == other.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Value);
    }
}
