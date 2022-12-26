using System.Collections.Generic;
using Luna.IDE.CodeEditing;
using Luna.IDE.Configuration;
using Luna.IDE.ProjectExploration;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.ProjectExploration;

internal class LastOpenedProjectFilesTest
{
    private DirectoryProjectItem _projectRoot;
    private Mock<IProject> _project;
    private Mock<IEnvironmentWindowsManager> _windowsManager;
    private Mock<IProjectItemOpenCommand> _projectItemOpenCommand;
    private Mock<IConfigStorage> _configStorage;
    private LastOpenedProjectFiles _lastOpenedProjectFiles;

    [SetUp]
    public void Setup()
    {
        _projectRoot = new DirectoryProjectItem("full path", null);
        _project = new Mock<IProject>();
        _project.SetupGet(x => x.Root).Returns(_projectRoot);
        _windowsManager = new Mock<IEnvironmentWindowsManager>();
        _projectItemOpenCommand = new Mock<IProjectItemOpenCommand>();
        _configStorage = new Mock<IConfigStorage>();
        _lastOpenedProjectFiles = new LastOpenedProjectFiles(_windowsManager.Object, _projectItemOpenCommand.Object, _configStorage.Object);
    }

    [Test]
    public void SaveOpenedFiles()
    {
        var model = new Mock<IEnvironmentWindowModel>();
        var editor = model.As<ICodeFileEditor>();
        editor.SetupGet(x => x.ProjectItem).Returns(new CodeFileProjectItem("file", _projectRoot, null));
        var view = new Mock<IEnvironmentWindowView>();
        var window = new EnvironmentWindow(123, model.Object, view.Object);
        _windowsManager.SetupGet(x => x.Windows).Returns(new[] { window });

        _lastOpenedProjectFiles.SaveOpenedFiles(_project.Object);

        _configStorage.Verify(x => x.Save(new LastOpenedProjectFilesPoco { ProjectFullPath = "full path", FilesRelativePathes = new List<string> { "file" } }), Times.Once());
    }

    [Test]
    public void RestoreLastOpenedFiles_NoLastOpenediles()
    {
        _configStorage.Setup(x => x.GetById<LastOpenedProjectFilesPoco>("full path")).Returns((LastOpenedProjectFilesPoco)null);

        _lastOpenedProjectFiles.RestoreLastOpenedFiles(_project.Object);

        _projectItemOpenCommand.Verify(x => x.Execute(It.IsAny<object>()), Times.Never());
    }

    [Test]
    public void RestoreLastOpenedFiles()
    {
        _configStorage.Setup(x => x.GetById<LastOpenedProjectFilesPoco>("full path")).Returns(new LastOpenedProjectFilesPoco
        {
            ProjectFullPath = "full path",
            FilesRelativePathes = new List<string> { "file1", "file2" }
        });
        var file1 = new CodeFileProjectItem("full path\\file1", null, null);
        var file2 = new CodeFileProjectItem("full path\\file2", null, null);
        _project.Setup(x => x.FindItemByPath("file1")).Returns(file1);
        _project.Setup(x => x.FindItemByPath("file2")).Returns(file2);

        _lastOpenedProjectFiles.RestoreLastOpenedFiles(_project.Object);

        _projectItemOpenCommand.Verify(x => x.Execute(new[] { file1, file2 }), Times.Once());
    }
}
