using System;
using System.Globalization;

namespace Luna.Runtime;

internal class FloatRuntimeValue : NumericRuntimeValue
{
    public override double FloatValue { get; }

    public FloatRuntimeValue(double value)
    {
        FloatValue = value;
    }

    public override string ToString()
    {
        return FloatValue.ToString(new NumberFormatInfo { NumberDecimalSeparator = "." });
    }

    public override bool Equals(object? obj)
    {
        return obj is FloatRuntimeValue value &&
               FloatValue == value.FloatValue;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FloatValue);
    }
}
