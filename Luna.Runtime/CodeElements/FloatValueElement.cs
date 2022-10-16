using System.Globalization;

namespace Luna.CodeElements;

public class FloatValueElement : ValueElement, IEquatable<FloatValueElement?>
{
    public double Value { get; }

    public FloatValueElement(double value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Value = value;
    }

    internal FloatValueElement(double value) : this(value, 0, 0) { }

    public override string ToString()
    {
        return Value.ToString(new NumberFormatInfo { NumberDecimalSeparator = "." });
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as FloatValueElement);
    }

    public bool Equals(FloatValueElement? other)
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
