using System.Threading.Tasks;
using Luna.IDE.Common;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;

namespace Luna.IDE.TextDiff;

public interface IProjectItemChanges
{
    ISingleTextDiff SingleTextDiff { get; }
    IDoubleTextDiff DoubleTextDiff { get; }
    Task MakeDiff(string? oldFileText, TextFileProjectItem newFile);
}

public class ProjectItemChanges : NotificationObject, IProjectItemChanges, IEnvironmentWindowModel
{
    private readonly ITextDiffEngine _textDiffEngine;
    private bool _inProgress;

    public ISingleTextDiff SingleTextDiff { get; }

    public IDoubleTextDiff DoubleTextDiff { get; }

    public string Header { get; private set; } = "";

    public bool InProgress
    {
        get => _inProgress;
        set { _inProgress = value; RaisePropertyChanged(() => InProgress); }
    }

    public ProjectItemChanges(
        ITextDiffEngine textDiffEngine,
        ISingleTextDiff singleTextDiff,
        IDoubleTextDiff doubleTextDiff)
    {
        _textDiffEngine = textDiffEngine;
        SingleTextDiff = singleTextDiff;
        DoubleTextDiff = doubleTextDiff;
    }

    public async Task MakeDiff(string? oldFileText, TextFileProjectItem newFile)
    {
        Header = $"Changes in {newFile.Name}";
        InProgress = true;
        var diffResult = await _textDiffEngine.GetDiffResultAsync(oldFileText ?? "", newFile.GetText());
        SingleTextDiff.MakeDiff(diffResult, oldFileText, newFile);
        DoubleTextDiff.MakeDiff(diffResult, oldFileText, newFile);
        InProgress = false;
    }
}
