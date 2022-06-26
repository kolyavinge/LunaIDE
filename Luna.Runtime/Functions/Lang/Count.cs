using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("count", "list")]
internal class Count : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var list = GetValueOrError<ListRuntimeValue>(0);
        return new IntegerRuntimeValue(list.Count);
    }
}
