using Luna.Runtime;

namespace Luna.Functions.Windows;

[EmbeddedFunctionDeclaration("app", "main_window")]
internal class App : EmbeddedFunction
{
    public override IRuntimeValue GetValue()
    {
        var mainWindow = GetValueOrError<ObjectRuntimeValue>(0).Get<System.Windows.Window>();
        mainWindow.Show();

        return new IntegerRuntimeValue(0);
    }
}
