using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("not", "x")]
internal class Not : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var x = arguments.GetValueOrError<BooleanRuntimeValue>(0);
        return new BooleanRuntimeValue(!x.Value);
    }
}
