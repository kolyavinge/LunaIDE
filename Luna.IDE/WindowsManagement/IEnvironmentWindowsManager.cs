using System.Collections.Generic;

namespace Luna.IDE.WindowsManagement;

public interface IEnvironmentWindowsManager
{
    event EventHandler? WindowOpened;
    event EventHandler? WindowClosed;
    IReadOnlyCollection<EnvironmentWindow> Windows { get; }
    EnvironmentWindow? SelectedWindow { get; }
    EnvironmentWindow? FindWindowById(object id);
    EnvironmentWindow OpenWindow(object id, IEnvironmentWindowModel model, IEnvironmentWindowView view);
    void ActivateWindow(EnvironmentWindow window);
    void CloseWindow(EnvironmentWindow window);
    void CloseAllWindows();
}
