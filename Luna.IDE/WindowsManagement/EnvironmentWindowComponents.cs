namespace Luna.IDE.WindowsManagement;

public class EnvironmentWindowComponents
{
    public IEnvironmentWindowModel Model { get; }

    public IEnvironmentWindowView View { get; }

    public EnvironmentWindowComponents(IEnvironmentWindowModel model, IEnvironmentWindowView view)
    {
        Model = model;
        View = view;
    }
}
