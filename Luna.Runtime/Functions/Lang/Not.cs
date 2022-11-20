using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("not", "x")]
internal class Not : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var x = GetValueOrError<BooleanRuntimeValue>(argumentValues, 0);
        return new BooleanRuntimeValue(!x.Value);
    }
}
