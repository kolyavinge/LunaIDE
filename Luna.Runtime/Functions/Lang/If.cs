using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunction("if", "condition trueFunc falseFunc")]
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
