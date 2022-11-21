using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("set", "variable value")]
internal class Set : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var variable = arguments.GetVariableOrError(0);
        var value = arguments.GetValueOrError<IRuntimeValue>(1);

        variable.SetValue(value);

        return value;
    }
}
