using Luna.IDE.App.Model;
using Luna.IDE.App.Mvvm;

namespace Luna.IDE.App.ViewModel;

public class OutputConsoleViewModel : NotificationObject
{
    public IOutputConsole OutputConsole { get; }

    public OutputConsoleViewModel(IOutputConsole outputConsole)
    {
        OutputConsole = outputConsole;
    }
}
