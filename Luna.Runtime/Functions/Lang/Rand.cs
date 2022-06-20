using System;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("rand", "from to")]
internal class Rand : EmbeddedFunction
{
    private readonly Random _rand = new();

    public override IRuntimeValue GetValue()
    {
        var from = GetValueOrError<IntegerRuntimeValue>(0).IntegerValue;
        var to = GetValueOrError<IntegerRuntimeValue>(1).IntegerValue;

        var result = _rand.Next(from, to + 1);

        return new IntegerRuntimeValue(result);
    }
}
