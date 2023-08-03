using Luna.IDE.WindowsManagement;

namespace Luna.IDE.Outputing;

public interface IOutputConsole
{
    IOutputArea OutputArea { get; }
}

public class OutputConsole : IOutputConsole, IEnvironmentWindowModel
{
    public string Header => "Output";

    public IOutputArea OutputArea { get; }

    public OutputConsole(IOutputArea outputArea)
    {
        OutputArea = outputArea;
    }
}
