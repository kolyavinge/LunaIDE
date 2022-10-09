using System.Windows;

namespace Luna.IDE.WindowsManagement;

public class EnvironmentWindow
{
    public object Id { get; }

    public IEnvironmentWindowModel Model { get; }

    public object View { get; }

    public bool IsLoaded { get; private set; }

    public event EventHandler? Loaded;

    public EnvironmentWindow(object id, IEnvironmentWindowModel model, object view)
    {
        Id = id;
        Model = model;
        View = view;
        if (View is FrameworkElement fe)
        {
            fe.Loaded += (s, e) => { IsLoaded = true; Loaded?.Invoke(this, EventArgs.Empty); };
            fe.Unloaded += (s, e) => { IsLoaded = false; };
        }
    }
}
