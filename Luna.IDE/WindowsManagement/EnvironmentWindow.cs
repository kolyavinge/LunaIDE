namespace Luna.IDE.WindowsManagement;

public class EnvironmentWindow
{
    public object Id { get; }

    public IEnvironmentWindowModel Model { get; }

    public IEnvironmentWindowView View { get; }

    public bool IsLoaded { get; private set; }

    public event EventHandler? Loaded;

    public EnvironmentWindow(object id, IEnvironmentWindowModel model, IEnvironmentWindowView view)
    {
        Id = id;
        Model = model;
        View = view;
        view.Loaded += (s, e) => { IsLoaded = true; Loaded?.Invoke(this, EventArgs.Empty); };
        view.Unloaded += (s, e) => { IsLoaded = false; };
    }
}
