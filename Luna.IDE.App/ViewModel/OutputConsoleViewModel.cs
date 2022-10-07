using Luna.IDE.Common;
using Luna.IDE.Outputing;

namespace Luna.IDE.App.ViewModel;

public class OutputConsoleViewModel : NotificationObject
{
    public IOutputConsole OutputConsole { get; }

    public OutputConsoleViewModel(IOutputConsole outputConsole)
    {
        OutputConsole = outputConsole;
    }
}
