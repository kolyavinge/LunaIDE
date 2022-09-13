using Luna.Collections;

namespace Luna.Runtime;

internal interface IRuntimeValue
{
    IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue>? argumentValues = null);

    string ToString();
}

internal class RuntimeValue : IRuntimeValue
{
    public virtual IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue>? argumentValues = null) => this;

    public override string ToString() => "";
}
