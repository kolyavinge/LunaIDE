using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("count", "list")]
internal class Count : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var list = arguments.GetValueOrError<ListRuntimeValue>(0);
        return new IntegerRuntimeValue(list.Count);
    }
}
