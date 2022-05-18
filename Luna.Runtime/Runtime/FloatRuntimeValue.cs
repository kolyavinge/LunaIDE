using System;
using System.Globalization;

namespace Luna.Runtime;

internal class FloatRuntimeValue : IRuntimeValue
{
    public double Value { get; }

    public FloatRuntimeValue(double value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString(new NumberFormatInfo { NumberDecimalSeparator = "." });
    }

    public override bool Equals(object? obj)
    {
        return obj is FloatRuntimeValue value &&
               Value == value.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }
}
