using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Luna.IDE.Common;
using Luna.Utils;

namespace Luna.IDE.WindowsManagement;

public interface IEnvironmentWindowsManager
{
    IReadOnlyCollection<EnvironmentWindow> Windows { get; }
    EnvironmentWindow? SelectedWindow { get; }
    EnvironmentWindow? FindWindowById(object id);
    EnvironmentWindow OpenWindow(object id, IEnvironmentWindowModel model, object view);
    void ActivateWindow(EnvironmentWindow window);
    void CloseWindow(EnvironmentWindow window);
    void CloseAllWindows();
}

public class EnvironmentWindowsManager : NotificationObject, IEnvironmentWindowsManager
{
    private readonly ObservableCollection<EnvironmentWindow> _windows = new();
    private EnvironmentWindow? _selectedWindow;

    public IReadOnlyCollection<EnvironmentWindow> Windows => _windows;

    public EnvironmentWindow? SelectedWindow
    {
        get => _selectedWindow;
        set
        {
            _selectedWindow = value;
            RaisePropertyChanged(() => SelectedWindow!);
        }
    }

    public EnvironmentWindow? FindWindowById(object id)
    {
        return Windows.FirstOrDefault(x => x.Id == id);
    }

    public EnvironmentWindow OpenWindow(object id, IEnvironmentWindowModel model, object view)
    {
        var window = new EnvironmentWindow(id, model, view);
        _windows.Add(window);

        return window;
    }

    public void ActivateWindow(EnvironmentWindow window)
    {
        SelectedWindow = Windows.FirstOrDefault(x => x == window);
    }

    public void CloseWindow(EnvironmentWindow window)
    {
        window.Model.Save();
        window.Model.Close();
        if (SelectedWindow == window)
        {
            var index = _windows.IndexOf(window);
            if (index < Windows.Count - 1) SelectedWindow = _windows[index + 1];
            else if (0 < index && index == Windows.Count - 1) SelectedWindow = _windows[index - 1];
            else SelectedWindow = null;
        }
        var component = Windows.First(x => x == window);
        _windows.Remove(component);
    }

    public void CloseAllWindows()
    {
        Windows.Each(x => x.Model.Save());
        Windows.Each(x => x.Model.Close());
        SelectedWindow = null;
        _windows.Clear();
    }
}
