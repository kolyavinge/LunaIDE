using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("count", "list")]
internal class Count : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var list = GetValueOrError<ListRuntimeValue>(argumentValues, 0);
        return new IntegerRuntimeValue(list.Count);
    }
}
