using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("set", "variable value")]
internal class Set : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var variable = GetVariableOrError(0);
        var value = GetValueOrError<IRuntimeValue>(1);

        variable.SetValue(value);

        return value;
    }
}
