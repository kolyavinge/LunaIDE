using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("rand", "from to")]
internal class Rand : EmbeddedFunction
{
    private readonly Random _rand = new();

    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var from = (int)arguments.GetValueOrError<IntegerRuntimeValue>(0).IntegerValue;
        var to = (int)arguments.GetValueOrError<IntegerRuntimeValue>(1).IntegerValue;

        var result = _rand.Next(from, to + 1);

        return new IntegerRuntimeValue(result);
    }
}
