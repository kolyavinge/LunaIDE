using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("rand", "from to")]
internal class Rand : EmbeddedFunction
{
    private readonly Random _rand = new();

    protected override IRuntimeValue InnerGetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var from = (int)GetValueOrError<IntegerRuntimeValue>(argumentValues, 0).IntegerValue;
        var to = (int)GetValueOrError<IntegerRuntimeValue>(argumentValues, 1).IntegerValue;

        var result = _rand.Next(from, to + 1);

        return new IntegerRuntimeValue(result);
    }
}
