﻿namespace Luna.Runtime;

internal class VariableRuntimeValue : RuntimeValue
{
    public IRuntimeValue Value { get; private set; }

    public VariableRuntimeValue()
    {
        Value = VoidRuntimeValue.Instance;
    }

    public VariableRuntimeValue(IRuntimeValue value)
    {
        Value = value;
    }

    public void SetValue(IRuntimeValue value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value?.ToString() ?? "";
    }

    public override bool Equals(object? obj)
    {
        return obj is VariableRuntimeValue value && Value.Equals(value.Value);
    }

    public override int GetHashCode()
    {
        return Value?.GetHashCode() ?? 0;
    }
}
