namespace Luna.IDE.WindowsManagement;

public interface IEnvironmentWindowModel
{
    string Header { get; }
}

public interface ICloseableEnvironmentWindow
{
    void Close();
}

public interface ISaveableEnvironmentWindow
{
    void Save();
}
