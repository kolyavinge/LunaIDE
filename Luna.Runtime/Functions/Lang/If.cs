using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("if", "condition true_func false_func")]
internal class If : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var condition = arguments.GetValueOrError<BooleanRuntimeValue>(0);
        if (condition.Value)
        {
            return arguments.GetValueOrError<IRuntimeValue>(1);
        }
        else
        {
            return arguments.GetValueOrError<IRuntimeValue>(2);
        }
    }
}
