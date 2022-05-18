using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunction("eq", "x y")]
internal class Eq : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var x = argumentValues[0];
        var y = argumentValues[1];

        var result = x.Equals(y);

        return new BooleanRuntimeValue(result);
    }
}
