using System;

namespace Luna.Runtime;

internal class StringRuntimeValue : RuntimeValue
{
    public string Value { get; }

    public StringRuntimeValue(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return $"'{Value}'";
    }

    public override bool Equals(object? obj)
    {
        return obj is StringRuntimeValue value &&
               Value == value.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }
}
