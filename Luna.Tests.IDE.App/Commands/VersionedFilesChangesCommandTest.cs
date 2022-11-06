using System.Text;
using Luna.IDE.App.Commands;
using Luna.IDE.App.Factories;
using Luna.IDE.ProjectExploration;
using Luna.IDE.TextDiff;
using Luna.IDE.Versioning;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.App.Commands;

internal class VersionedFilesChangesCommandTest
{
    private Mock<VersionControl.Core.IVersionControlRepository> _versionControlRepository;
    private Mock<IProject> _project;
    private Mock<ISelectedProject> _selectedProject;
    private Mock<IEnvironmentWindowsManager> _windowsManager;
    private Mock<IEnvironmentWindowsFactory> _environmentWindowsFactory;
    private Mock<IProjectItemChanges> _projectItemChanges;
    private Mock<IEnvironmentWindowModel> _windowModel;
    private object _view;
    private VersionedFilesChangesCommand _command;

    [SetUp]
    public void Setup()
    {
        _versionControlRepository = new Mock<VersionControl.Core.IVersionControlRepository>();
        _project = new Mock<IProject>();
        _selectedProject = new Mock<ISelectedProject>();
        _selectedProject.SetupGet(x => x.Project).Returns(_project.Object);
        _windowsManager = new Mock<IEnvironmentWindowsManager>();
        _environmentWindowsFactory = new Mock<IEnvironmentWindowsFactory>();
        _projectItemChanges = new Mock<IProjectItemChanges>();
        _windowModel = _projectItemChanges.As<IEnvironmentWindowModel>();
        _view = new object();
        _command = new VersionedFilesChangesCommand(_selectedProject.Object, _windowsManager.Object, _environmentWindowsFactory.Object);
    }

    [Test]
    public void Execute()
    {
        var codeFile = new CodeFileProjectItem("path", null, null);
        _project.Setup(x => x.FindItemByPath("path")).Returns(codeFile);
        var versionedRepositoryFile = new VersionControl.Core.VersionedFile(1, "path", "path", 1, VersionControl.Core.FileActionKind.Add);
        _versionControlRepository.Setup(x => x.GetActualFileContent(versionedRepositoryFile)).Returns(Encoding.UTF8.GetBytes("old text"));
        var versionedFiles = new VersionedFile[]
        {
            new(_versionControlRepository.Object, versionedRepositoryFile)
        };
        _windowsManager.Setup(x => x.FindWindowById("Changes_path")).Returns((EnvironmentWindow)null);
        _environmentWindowsFactory.Setup(x => x.MakeWindowFor(typeof(ProjectItemChanges))).Returns(new EnvironmentWindowComponents(_windowModel.Object, _view));
        var environmentWindow = new EnvironmentWindow("Changes_path", _windowModel.Object, _view);
        _windowsManager.Setup(x => x.OpenWindow("Changes_path", _windowModel.Object, _view)).Returns(environmentWindow);

        _command.Execute(versionedFiles);

        _projectItemChanges.Verify(x => x.MakeDiff("old text", codeFile), Times.Once());
        _windowsManager.Verify(x => x.OpenWindow("Changes_path", _windowModel.Object, _view), Times.Once());
        _windowsManager.Verify(x => x.ActivateWindow(environmentWindow), Times.Once());
    }
}
