using System;

namespace Luna.Runtime;

internal class IntegerRuntimeValue : NumericRuntimeValue
{

    public IntegerRuntimeValue(int value) : base(value)
    {
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
