using System.Collections.Generic;
using System.Linq;
using Luna.IDE.Common;
using Luna.Utils;
using WindowsEnvironment.Model;

namespace Luna.IDE.WindowsManagement;

internal class EnvironmentWindowsManager : NotificationObject, IEnvironmentWindowsManager
{
    record EnvironmentWindowAdditional(string PanelName, string TabName);

    private readonly List<EnvironmentWindow> _windows = new();
    private readonly Dictionary<EnvironmentWindow, EnvironmentWindowAdditional> _additional = new();
    private readonly IFlexWindowsEnvironment _flexEnvironment;
    private EnvironmentWindow? _selectedWindow;

    public event EventHandler? WindowOpened;
    public event EventHandler? WindowClosed;

    public IReadOnlyCollection<EnvironmentWindow> Windows => _windows;

    public EnvironmentWindow? SelectedWindow
    {
        get => _selectedWindow;
        private set
        {
            _selectedWindow = value;
            RaisePropertyChanged(() => SelectedWindow!);
        }
    }

    public EnvironmentWindowsManager(IFlexWindowsEnvironment flexEnvironment)
    {
        _flexEnvironment = flexEnvironment;
    }

    public EnvironmentWindow? FindWindowById(object id)
    {
        return _windows.FirstOrDefault(x => x.Id.Equals(id));
    }

    public EnvironmentWindow OpenWindow(object id, IEnvironmentWindowModel model, IEnvironmentWindowView view)
    {
        var window = new EnvironmentWindow(id, model, view);
        var (panel, tab) = _flexEnvironment.SetPanelPosition(MainPanel.Name, PanelPosition.Middle, new()
        {
            Header = new() { SourceObject = model, PropertyName = "Header" },
            View = view.Content,
            CloseCallback = () => CloseWindow(window)
        });
        _windows.Add(window);
        _additional.Add(window, new(panel.Name, tab.Name));
        WindowOpened?.Invoke(this, EventArgs.Empty);

        return window;
    }

    public void ActivateWindow(EnvironmentWindow window)
    {
        if (_additional.TryGetValue(window, out var additional))
        {
            SelectedWindow = window;
            _flexEnvironment.SelectTab(additional.PanelName, additional.TabName);
        }
        else
        {
            SelectedWindow = null;
        }
    }

    public void CloseWindow(EnvironmentWindow window)
    {
        if (window.Model is ISaveableEnvironmentWindow saveable) saveable.Save();
        if (window.Model is ICloseableEnvironmentWindow closeable) closeable.Close();
        if (SelectedWindow == window)
        {
            var index = _windows.IndexOf(window);
            if (index < _windows.Count - 1) SelectedWindow = _windows[index + 1];
            else if (0 < index && index == _windows.Count - 1) SelectedWindow = _windows[index - 1];
            else SelectedWindow = null;
        }
        var component = _windows.First(x => x == window);
        _windows.Remove(component);
        _additional.Remove(component);
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }

    public void CloseAllWindows()
    {
        _windows.Where(x => x.Model is ISaveableEnvironmentWindow).Each(x => ((ISaveableEnvironmentWindow)x.Model).Save());
        _windows.Where(x => x.Model is ICloseableEnvironmentWindow).Each(x => ((ICloseableEnvironmentWindow)x.Model).Close());
        foreach (var window in _windows.Where(x => x.Model is ICloseableEnvironmentWindow).ToList())
        {
            var additional = _additional[window];
            _flexEnvironment.RemoveTab(additional.PanelName, additional.TabName, RemoveTabMode.Close);
        }
        SelectedWindow = null;
        _windows.Clear();
        _additional.Clear();
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }
}
