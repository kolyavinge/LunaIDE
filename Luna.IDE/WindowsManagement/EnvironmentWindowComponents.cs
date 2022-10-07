namespace Luna.IDE.WindowsManagement;

public class EnvironmentWindowComponents
{
    public IEnvironmentWindowModel Model { get; }

    public object View { get; }

    public EnvironmentWindowComponents(IEnvironmentWindowModel model, object view)
    {
        Model = model;
        View = view;
    }
}
