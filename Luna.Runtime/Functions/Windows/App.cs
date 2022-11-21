using System.Collections.Generic;
using Luna.Runtime;

namespace Luna.Functions.Windows;

static class AppWindowsCollection
{
    public static readonly List<IAppWindow> Windows = new();
}

[EmbeddedFunctionDeclaration("app", "main_window")]
internal class App : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var mainWindow = arguments.GetValueOrError<ObjectRuntimeValue>(0).Get<AppWindow>();
        AppWindowsCollection.Windows.Add(mainWindow);
        mainWindow.Show();

        return new IntegerRuntimeValue(0);
    }
}
