using System.Windows;

namespace Luna.IDE.WindowsManagement;

public interface IEnvironmentWindowView
{
    event EventHandler? Loaded;

    event EventHandler? Unloaded;

    object Content { get; }

    void Focus();
}

public class EnvironmentWindowView : IEnvironmentWindowView
{
    private readonly FrameworkElement _frameworkElement;

    public event EventHandler? Loaded;

    public event EventHandler? Unloaded;

    public object Content => _frameworkElement;

    public EnvironmentWindowView(FrameworkElement frameworkElement)
    {
        _frameworkElement = frameworkElement;
        _frameworkElement.Loaded += (s, e) => { Loaded?.Invoke(this, EventArgs.Empty); };
        _frameworkElement.Unloaded += (s, e) => { Unloaded?.Invoke(this, EventArgs.Empty); };
    }

    public void Focus()
    {
        _frameworkElement.Focus();
    }
}
