using System.Threading.Tasks;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;

namespace Luna.IDE.TextDiff;

public interface IProjectItemChanges
{
    ISingleTextDiff SingleTextDiff { get; }
    Task MakeDiff(string? oldFileText, TextFileProjectItem newFile);
}

public class ProjectItemChanges : IProjectItemChanges, IEnvironmentWindowModel
{
    public ISingleTextDiff SingleTextDiff { get; }

    public string Header { get; private set; } = "";

    public ProjectItemChanges(ISingleTextDiff singleTextDiff)
    {
        SingleTextDiff = singleTextDiff;
    }

    public async Task MakeDiff(string? oldFileText, TextFileProjectItem newFile)
    {
        Header = $"Changes in {newFile.Name}";
        await SingleTextDiff.MakeDiff(oldFileText, newFile);
    }
}
