namespace Luna.IDE.WindowsManagement;

public interface IEnvironmentWindowsFactory
{
    EnvironmentWindowComponents MakeWindowFor(Type modelType);
}
