using Luna.Runtime;

namespace Luna.Functions.Windows;

[EmbeddedFunctionDeclaration("update_window", "window")]
internal class UpdateWindow : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var window = arguments.GetValueOrError<ObjectRuntimeValue>(0).Get<System.Windows.Window>();
        window.InvalidateVisual();

        return new ObjectRuntimeValue(window);
    }
}
