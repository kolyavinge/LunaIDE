using System.Linq;
using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.CodeEditing;
using Luna.IDE.Outputing;
using Luna.IDE.TextDiff;
using Luna.IDE.Versioning;
using Luna.IDE.WindowsManagement;
using Luna.Output;

namespace Luna.IDE.App.Commands.CodeFileEditor;

public interface ICodeFileEditorChangesCommand : ICommand { }

public class CodeFileEditorChangesCommand : Command<ICodeFileEditor>, ICodeFileEditorChangesCommand
{
    private readonly IProjectRepository _projectRepository;
    private readonly IEnvironmentWindowsManager _windowsManager;
    private readonly IEnvironmentWindowsFactory _environmentWindowsFactory;
    private readonly IOutputArea _outputArea;

    public CodeFileEditorChangesCommand(
        IProjectRepository projectRepository,
        IEnvironmentWindowsManager windowsManager,
        IEnvironmentWindowsFactory environmentWindowsFactory,
        IOutputArea outputArea)
    {
        _projectRepository = projectRepository;
        _windowsManager = windowsManager;
        _environmentWindowsFactory = environmentWindowsFactory;
        _outputArea = outputArea;
    }

    protected override void Execute(ICodeFileEditor editor)
    {
        var versionedFile =
            _projectRepository.Included.AllFiles.FirstOrDefault(x => x.FullPath == editor.ProjectItem.FullPath) ??
            _projectRepository.Excluded.AllFiles.FirstOrDefault(x => x.FullPath == editor.ProjectItem.FullPath);

        if (versionedFile != null)
        {
            var windowId = $"Changes_{versionedFile.RelativePath}";
            var window = _windowsManager.FindWindowById(windowId);
            if (window == null)
            {
                var components = _environmentWindowsFactory.MakeWindowFor(typeof(ProjectItemChanges));
                ((IProjectItemChanges)components.Model).MakeDiff(versionedFile.LastCommitContent, editor.ProjectItem);
                window = _windowsManager.OpenWindow(windowId, components.Model, components.View);
            }
            _windowsManager.ActivateWindow(window);
        }
        else
        {
            _outputArea.SendMessage(new OutputMessage(new OutputMessageItem[]
            {
                new($"No uncommited changes in ", OutputMessageKind.Text),
                new(editor.ProjectItem.Name, OutputMessageKind.Info)
            }));
        }
    }
}
