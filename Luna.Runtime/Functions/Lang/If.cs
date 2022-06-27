using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("if", "condition true_func false_func")]
internal class If : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var condition = GetValueOrError<BooleanRuntimeValue>(argumentValues, 0);
        if (condition.Value)
        {
            return GetValueOrError<IRuntimeValue>(argumentValues, 1);
        }
        else
        {
            return GetValueOrError<IRuntimeValue>(argumentValues, 2);
        }
    }
}
