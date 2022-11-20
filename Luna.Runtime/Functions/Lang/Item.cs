using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("item", "index list")]
internal class Item : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var index = (int)GetValueOrError<NumericRuntimeValue>(argumentValues, 0).IntegerValue;
        var list = GetValueOrError<ListRuntimeValue>(argumentValues, 1);

        if (index < 0 || index >= list.Count)
        {
            throw new RuntimeException("Index must be within list items range.");
        }

        return list.GetItem(index);
    }
}
