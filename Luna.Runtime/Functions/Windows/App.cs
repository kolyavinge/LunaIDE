using Luna.Runtime;

namespace Luna.Functions.Windows;

[EmbeddedFunctionDeclaration("app", "main_window")]
internal class App : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var mainWindow = arguments.GetValueOrError<ObjectRuntimeValue>(0).Get<System.Windows.Window>();
        mainWindow.Show();

        return new IntegerRuntimeValue(0);
    }
}
