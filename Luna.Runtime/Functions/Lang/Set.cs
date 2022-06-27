using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("set", "variable value")]
internal class Set : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var variable = GetVariableOrError(argumentValues, 0);
        var value = GetValueOrError<IRuntimeValue>(argumentValues, 1);

        variable.SetValue(value);

        return value;
    }
}
