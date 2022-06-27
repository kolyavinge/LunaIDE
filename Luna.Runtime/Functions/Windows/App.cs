using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Windows;

[EmbeddedFunctionDeclaration("app", "main_window")]
internal class App : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var mainWindow = GetValueOrError<ObjectRuntimeValue>(argumentValues, 0).Get<System.Windows.Window>();
        mainWindow.Show();

        return new IntegerRuntimeValue(0);
    }
}
