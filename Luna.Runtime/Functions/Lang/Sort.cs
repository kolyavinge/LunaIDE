using System.Linq;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("sort", "list compare_func")]
internal class Sort : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var list = GetValueOrError<ListRuntimeValue>(argumentValues, 0);
        var compareFunc = GetFunctionOrError(argumentValues, 1);

        var comparer = new ListRuntimeValueComparer(compareFunc);

        try
        {
            var result = list.OrderBy(x => x, comparer).ToList();
            return new ListRuntimeValue(result);
        }
        catch (InvalidOperationException e)
        {
            throw e.InnerException!;
        }
    }
}
