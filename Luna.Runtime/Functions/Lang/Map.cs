using System.Collections.Generic;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("map", "list func")]
internal class Map : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var list = arguments.GetValueOrError<ListRuntimeValue>(0);
        var func = arguments.GetFunctionOrError(1);

        var result = new List<IRuntimeValue>();

        foreach (var item in list)
        {
            var value = func.GetValue(new(new[] { item }));
            result.Add(value);
        }

        return new ListRuntimeValue(result);
    }
}
