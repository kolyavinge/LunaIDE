namespace Luna.IDE.WindowsManagement;

public interface IEnvironmentWindowModel
{
    string Header { get; }
    void Save();
    void Close();
}
