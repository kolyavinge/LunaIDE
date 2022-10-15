using System.Collections.Generic;
using System.Linq;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;
using VersionControl.Core;

namespace Luna.IDE.CodeEditing;

public interface ICodeEditorUndoChangesLogic
{
    void UndoTextChanges(IReadOnlyCollection<VersionedFile> versionedFiles);
}

public class CodeEditorUndoChangesLogic : ICodeEditorUndoChangesLogic
{
    private readonly ICodeModelUpdater _codeModelUpdater;
    private readonly IEnvironmentWindowsManager _environmentWindowsManager;

    public CodeEditorUndoChangesLogic(
        ICodeModelUpdater codeModelUpdater,
        IEnvironmentWindowsManager environmentWindowsManager)
    {
        _codeModelUpdater = codeModelUpdater;
        _environmentWindowsManager = environmentWindowsManager;
    }

    public void UndoTextChanges(IReadOnlyCollection<VersionedFile> versionedFiles)
    {
        var editors = _environmentWindowsManager.Windows
            .Where(x => x.Model is ICodeFileEditor)
            .ToDictionary(k => ((ICodeFileEditor)k.Model).ProjectItem.FullPath, v => v.Model);

        var filesToClose = versionedFiles.Where(x => x.ActionKind is FileActionKind.Add or FileActionKind.Replace or FileActionKind.ModifyAndReplace).ToList();
        var filesToUndoTextChanges = versionedFiles.Where(x => x.ActionKind is FileActionKind.Modify).ToList();

        foreach (var file in filesToClose)
        {
            if (editors.ContainsKey(file.FullPath))
            {
                editors[file.FullPath].Close();
            }
        }

        foreach (var file in filesToUndoTextChanges)
        {
            if (editors.ContainsKey(file.FullPath))
            {
                ((ICodeFileEditor)editors[file.FullPath]).UndoTextChanges();
            }
        }

        if (filesToUndoTextChanges.Any())
        {
            _codeModelUpdater.UpdateRequest();
        }
    }
}
