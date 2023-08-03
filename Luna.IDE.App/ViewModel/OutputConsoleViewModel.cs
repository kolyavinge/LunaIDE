using Luna.IDE.Common;
using Luna.IDE.Outputing;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.ViewModel;

[EnvironmentWindowFor(typeof(OutputConsole))]
public class OutputConsoleViewModel : NotificationObject
{
    public IOutputConsole OutputConsole { get; }

    public OutputConsoleViewModel(IOutputConsole outputConsole)
    {
        OutputConsole = outputConsole;
    }
}
