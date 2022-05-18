using System;

namespace Luna.Runtime;

internal class BooleanRuntimeValue : IRuntimeValue
{
    public bool Value { get; }

    public BooleanRuntimeValue(bool value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString().ToLower();
    }

    public override bool Equals(object? obj)
    {
        return obj is BooleanRuntimeValue value &&
               Value == value.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }
}
