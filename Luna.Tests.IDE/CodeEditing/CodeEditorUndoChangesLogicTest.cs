using Luna.IDE.CodeEditing;
using Luna.IDE.Versioning;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;
using FileActionKind = VersionControl.Core.FileActionKind;

namespace Luna.Tests.IDE.CodeEditing;

internal class CodeEditorUndoChangesLogicTest
{
    private Mock<VersionControl.Core.IVersionControlRepository> _versionControlRepository;
    private Mock<IEnvironmentWindowModel> _environmentWindowModel;
    private Mock<ICodeFileEditor> _codeFileEditor;
    private Mock<IEnvironmentWindowsManager> _windowsManager;
    private Mock<ICodeModelUpdater> _codeModelUpdater;
    private Mock<IEnvironmentWindowView> _view;
    private CodeEditorUndoChangesLogic _undoChangesLogic;

    [SetUp]
    public void Setup()
    {
        _versionControlRepository = new Mock<VersionControl.Core.IVersionControlRepository>();
        _environmentWindowModel = new Mock<IEnvironmentWindowModel>();
        _codeFileEditor = _environmentWindowModel.As<ICodeFileEditor>();
        _windowsManager = new Mock<IEnvironmentWindowsManager>();
        _codeModelUpdater = new Mock<ICodeModelUpdater>();
        _view = new Mock<IEnvironmentWindowView>();
        _undoChangesLogic = new CodeEditorUndoChangesLogic(_codeModelUpdater.Object, _windowsManager.Object);
    }

    [Test]
    public void AddedFile_CloseEditor()
    {
        _codeFileEditor.SetupGet(x => x.ProjectItem).Returns(new CodeFileProjectItem("c:\\file", null, null));
        _windowsManager.Setup(x => x.Windows).Returns(new[] { new EnvironmentWindow("editor", _environmentWindowModel.Object, _view.Object) });
        var files = new VersionedFile[] { new(_versionControlRepository.Object, new(1, "c:\\file", "file", 1, FileActionKind.Add)) };

        _undoChangesLogic.UndoTextChanges(files);

        _codeFileEditor.Verify(x => x.Close(), Times.Once());
        _codeModelUpdater.Verify(x => x.UpdateRequest(), Times.Never());
    }

    [Test]
    public void ReplacedFile_CloseEditor()
    {
        _codeFileEditor.SetupGet(x => x.ProjectItem).Returns(new CodeFileProjectItem("c:\\file", null, null));
        _windowsManager.Setup(x => x.Windows).Returns(new[] { new EnvironmentWindow("editor", _environmentWindowModel.Object, _view.Object) });
        var files = new VersionedFile[] { new(_versionControlRepository.Object, new(1, "c:\\file", "file", 1, FileActionKind.Replace)) };

        _undoChangesLogic.UndoTextChanges(files);

        _codeFileEditor.Verify(x => x.Close(), Times.Once());
        _codeModelUpdater.Verify(x => x.UpdateRequest(), Times.Never());
    }

    [Test]
    public void ModifiedAndReplacedFile_CloseEditor()
    {
        _codeFileEditor.SetupGet(x => x.ProjectItem).Returns(new CodeFileProjectItem("c:\\file", null, null));
        _windowsManager.Setup(x => x.Windows).Returns(new[] { new EnvironmentWindow("editor", _environmentWindowModel.Object, _view.Object) });
        var files = new VersionedFile[] { new(_versionControlRepository.Object, new(1, "c:\\file", "file", 1, FileActionKind.ModifyAndReplace)) };

        _undoChangesLogic.UndoTextChanges(files);

        _codeFileEditor.Verify(x => x.Close(), Times.Once());
        _codeModelUpdater.Verify(x => x.UpdateRequest(), Times.Never());
    }

    [Test]
    public void ModifiedFile_UndoTextChanges()
    {
        _codeFileEditor.SetupGet(x => x.ProjectItem).Returns(new CodeFileProjectItem("c:\\file", null, null));
        _windowsManager.Setup(x => x.Windows).Returns(new[] { new EnvironmentWindow("editor", _environmentWindowModel.Object, _view.Object) });
        var files = new VersionedFile[] { new(_versionControlRepository.Object, new(1, "c:\\file", "file", 1, FileActionKind.Modify)) };

        _undoChangesLogic.UndoTextChanges(files);

        _codeFileEditor.Verify(x => x.UndoTextChanges(), Times.Once());
        _codeModelUpdater.Verify(x => x.UpdateRequest(), Times.Once());
    }
}
