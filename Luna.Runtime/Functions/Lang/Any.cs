using System.Linq;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("any", "list")]
internal class Any : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var list = GetValueOrError<ListRuntimeValue>(argumentValues, 0);
        return new BooleanRuntimeValue(list.Any());
    }
}
