using Luna.Collections;

namespace Luna.Runtime;

internal interface IRuntimeValue
{
    IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue>? argumentValues = null);
}

internal class RuntimeValue : IRuntimeValue
{
    public virtual IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue>? argumentValues = null)
    {
        return this;
    }
}
