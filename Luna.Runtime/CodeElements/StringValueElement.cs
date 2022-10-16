namespace Luna.CodeElements;

public class StringValueElement : ValueElement, IEquatable<StringValueElement?>
{
    public string Value { get; }

    public StringValueElement(string value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Value = value;
    }

    internal StringValueElement(string value) : this(value, 0, 0) { }

    public override string ToString()
    {
        return $"'{Value}'";
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as StringValueElement);
    }

    public bool Equals(StringValueElement? other)
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
