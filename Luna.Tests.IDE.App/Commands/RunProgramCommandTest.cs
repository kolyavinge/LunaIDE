using System.Collections.Generic;
using Luna.IDE.App.Commands;
using Luna.IDE.App.Model;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Luna.Runtime;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.App.Commands;

internal class RunProgramCommandTest
{
    private Mock<IFileSystem> _fileSystem;
    private Mock<IInterpreter> _interpreter;
    private Project _project;
    private Mock<IProjectExplorer> _projectExplorer;
    private Mock<IEnvironmentWindowsManager> _windowsManager;
    private Mock<IOutputArea> _outputArea;
    private RunProgramCommand _command;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();
        _interpreter = new Mock<IInterpreter>();
        _projectExplorer = new Mock<IProjectExplorer>();
        _project = new Project("", _fileSystem.Object);
        _projectExplorer.SetupGet(x => x.Project).Returns(_project);
        _windowsManager = new Mock<IEnvironmentWindowsManager>();
        _windowsManager.SetupGet(x => x.Windows).Returns(new List<EnvironmentWindow>());
        _outputArea = new Mock<IOutputArea>();
        _command = new RunProgramCommand(_interpreter.Object, _projectExplorer.Object, _windowsManager.Object, _outputArea.Object);
    }

    [Test]
    public void ProjectNotSelected_NoRun()
    {
        _projectExplorer.SetupGet(x => x.Project).Returns((Project)null);
        _command.Execute(null);
        _interpreter.Verify(x => x.Run(_project, _outputArea.Object), Times.Never());
    }

    [Test]
    public void ProjectSelected_Run()
    {
        _command.Execute(null);
        _interpreter.Verify(x => x.Run(_project, _outputArea.Object), Times.Once());
    }

    [Test]
    public void SaveOpenedEditorsBeforeRun()
    {
        var model1 = new Mock<IEnvironmentWindowModel>();
        var model2 = new Mock<IEnvironmentWindowModel>();
        var model3 = new Mock<IEnvironmentWindowModel>();
        var windows = new List<EnvironmentWindow>
        {
            new("1", model1.Object, new object()),
            new("1", model2.Object, new object()),
            new("1", model3.Object, new object())
        };
        _windowsManager.SetupGet(x => x.Windows).Returns(windows);

        _command.Execute(null);

        model1.Verify(x => x.Save(), Times.Once());
        model2.Verify(x => x.Save(), Times.Once());
        model3.Verify(x => x.Save(), Times.Once());
    }

    [Test]
    public void ClearOutputArea()
    {
        _command.Execute(null);
        _outputArea.Verify(x => x.Clear(), Times.Once());
    }
}
