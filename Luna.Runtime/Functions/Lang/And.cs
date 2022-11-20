using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("and", "x y")]
internal class And : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        return new BooleanRuntimeValue(
            GetValueOrError<BooleanRuntimeValue>(argumentValues, 0).Value && GetValueOrError<BooleanRuntimeValue>(argumentValues, 1).Value);
    }
}
