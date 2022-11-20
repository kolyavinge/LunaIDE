using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("or", "x y")]
internal class Or : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        return new BooleanRuntimeValue(
            GetValueOrError<BooleanRuntimeValue>(argumentValues, 0).Value || GetValueOrError<BooleanRuntimeValue>(argumentValues, 1).Value);
    }
}
