using System;

namespace Luna.Runtime;

internal class IntegerRuntimeValue : RuntimeValue
{
    public int Value { get; }

    public IntegerRuntimeValue(int value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj is IntegerRuntimeValue value &&
               Value == value.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }
}
