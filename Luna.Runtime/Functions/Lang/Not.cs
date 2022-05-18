using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunction("not", "x")]
internal class Not : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var x = GetValueOrError<BooleanRuntimeValue>(argumentValues, 0);
        return new BooleanRuntimeValue(!x.Value);
    }
}
