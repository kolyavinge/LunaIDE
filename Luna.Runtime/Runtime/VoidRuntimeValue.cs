﻿namespace Luna.Runtime;

internal class VoidRuntimeValue : IRuntimeValue
{
    public override string ToString()
    {
        return "void";
    }

    public override bool Equals(object? obj)
    {
        return obj is VoidRuntimeValue;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}
