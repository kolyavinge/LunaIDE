using System.Collections.Generic;
using System.Linq;
using Luna.IDE.Versioning;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;
using FileActionKind = VersionControl.Core.FileActionKind;

namespace Luna.IDE.CodeEditing;

internal interface ICodeEditorUndoChangesLogic
{
    void UndoTextChanges(IReadOnlyCollection<VersionedFile> versionedFiles);
}

internal class CodeEditorUndoChangesLogic : ICodeEditorUndoChangesLogic
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
            .ToDictionary(k => ((ICodeFileEditor)k.Model).ProjectItem.FullPath, v => (ICodeFileEditor)v.Model);

        var filesToClose = versionedFiles.Where(x => x.ActionKind is FileActionKind.Add or FileActionKind.Replace or FileActionKind.ModifyAndReplace).ToList();
        var filesToUndoTextChanges = versionedFiles.Where(x => x.ActionKind is FileActionKind.Modify).ToList();

        foreach (var file in filesToClose)
        {
            if (editors.TryGetValue(file.FullPath, out var value))
            {
                value.Close();
            }
        }

        foreach (var file in filesToUndoTextChanges)
        {
            if (editors.TryGetValue(file.FullPath, out var value))
            {
                value.UndoTextChanges();
            }
        }

        if (filesToUndoTextChanges.Any())
        {
            _codeModelUpdater.Request();
        }
    }
}
