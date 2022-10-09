namespace Luna.IDE.WindowsManagement;

[AttributeUsage(AttributeTargets.Class)]
public class EnvironmentWindowForAttribute : Attribute
{
    public Type ModelType { get; }

    public EnvironmentWindowForAttribute(Type modelType)
    {
        ModelType = modelType;
    }
}
