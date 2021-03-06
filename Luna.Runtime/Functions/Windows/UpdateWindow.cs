using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Windows;

[EmbeddedFunctionDeclaration("update_window", "window")]
internal class UpdateWindow : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var window = GetValueOrError<ObjectRuntimeValue>(argumentValues, 0).Get<System.Windows.Window>();
        window.InvalidateVisual();

        return new ObjectRuntimeValue(window);
    }
}
