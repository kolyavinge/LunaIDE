namespace Luna.CodeElements;

public class BooleanValueElement : ValueElement, IEquatable<BooleanValueElement?>
{
    public bool Value { get; }

    public BooleanValueElement(bool value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Value = value;
    }

    internal BooleanValueElement(bool value) : this(value, 0, 0) { }

    public override string ToString()
    {
        return Value.ToString().ToLower();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as BooleanValueElement);
    }

    public bool Equals(BooleanValueElement? other)
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
