using System;
using System.Globalization;

namespace Luna.Runtime;

internal class FloatRuntimeValue : NumericRuntimeValue
{
    public FloatRuntimeValue(double value) : base(value)
    {
    }

    public override string ToString()
    {
        return FloatValue.ToString(new NumberFormatInfo { NumberDecimalSeparator = "." });
    }

    public override bool Equals(object? obj)
    {
        return obj is FloatRuntimeValue value &&
               Math.Abs(FloatValue - value.FloatValue) < 0.00001;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FloatValue);
    }
}
