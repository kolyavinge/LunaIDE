using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("if", "condition true_func false_func")]
internal class If : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var condition = GetValueOrError<BooleanRuntimeValue>(0);
        if (condition.Value)
        {
            return GetValueOrError<IRuntimeValue>(1);
        }
        else
        {
            return GetValueOrError<IRuntimeValue>(2);
        }
    }
}
