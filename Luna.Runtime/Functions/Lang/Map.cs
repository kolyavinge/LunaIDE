using System.Collections.Generic;
using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("map", "list func")]
internal class Map : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var list = GetValueOrError<ListRuntimeValue>(argumentValues, 0);
        var func = GetFunctionOrError(argumentValues, 1);

        var result = new List<IRuntimeValue>();

        foreach (var item in list)
        {
            var value = func.GetValue(new(new[] { item }));
            result.Add(value);
        }

        return new ListRuntimeValue(result);
    }
}
