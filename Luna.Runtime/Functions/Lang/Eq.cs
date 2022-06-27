using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("eq", "x y")]
internal class Eq : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var x = GetValueOrError<IRuntimeValue>(argumentValues, 0);
        var y = GetValueOrError<IRuntimeValue>(argumentValues, 1);

        var result = x.Equals(y);

        return new BooleanRuntimeValue(result);
    }
}
