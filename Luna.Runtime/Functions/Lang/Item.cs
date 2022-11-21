using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("item", "index list")]
internal class Item : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var index = (int)arguments.GetValueOrError<NumericRuntimeValue>(0).IntegerValue;
        var list = arguments.GetValueOrError<ListRuntimeValue>(1);

        if (index < 0 || index >= list.Count)
        {
            throw new RuntimeException("Index must be within list items range.");
        }

        return list.GetItem(index);
    }
}
