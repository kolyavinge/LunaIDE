using System.Collections.Generic;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("filter", "list func")]
internal class Filter : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var list = GetValueOrError<ListRuntimeValue>(argumentValues, 0);
        var func = GetFunctionOrError(argumentValues, 1);

        var result = new List<IRuntimeValue>();

        foreach (var item in list)
        {
            var filterResult = (BooleanRuntimeValue)func.GetValue(new(new[] { item }));
            if (filterResult.Value) result.Add(item);
        }

        return new ListRuntimeValue(result);
    }
}
