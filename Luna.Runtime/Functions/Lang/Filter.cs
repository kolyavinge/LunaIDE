using System.Collections.Generic;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("filter", "list func")]
internal class Filter : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var list = GetValueOrError<ListRuntimeValue>(0);
        var func = GetFunctionOrError(1);

        var result = new List<IRuntimeValue>();

        foreach (var item in list)
        {
            var filterResult = (BooleanRuntimeValue)func.GetValue(new(new[] { item }));
            if (filterResult.Value) result.Add(item);
        }

        return new ListRuntimeValue(result);
    }
}
