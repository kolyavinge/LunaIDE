using System;

namespace Luna.Runtime;

internal class IntegerRuntimeValue : NumericRuntimeValue
{
    public int IntegerValue { get; }

    public override double FloatValue => IntegerValue;

    public IntegerRuntimeValue(int value)
    {
        IntegerValue = value;
    }

    public override string ToString()
    {
        return IntegerValue.ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj is IntegerRuntimeValue value &&
               IntegerValue == value.IntegerValue;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IntegerValue);
    }
}
