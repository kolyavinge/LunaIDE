﻿using System.Collections.Generic;
using Luna.IDE.App.Commands;
using Luna.IDE.Outputing;
using Luna.IDE.ProjectExploration;
using Luna.IDE.WindowsManagement;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Luna.Runtime;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.App.Commands;

internal class RunProgramCommandTest
{
    private Mock<IFileSystem> _fileSystem;
    private Project _project;
    private Mock<ISelectedProject> _selectedProject;
    private Mock<IEnvironmentWindowsManager> _windowsManager;
    private Mock<IOutputArea> _outputArea;
    private Mock<IEnvironmentWindowView> _view;
    private Mock<IInterpreter> _interpreter;
    private Mock<IInterpreterFactory> _interpreterFactory;
    private RunProgramCommand _command;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();
        _selectedProject = new Mock<ISelectedProject>();
        _project = new Project("", _fileSystem.Object);
        _selectedProject.SetupGet(x => x.Project).Returns(_project);
        _windowsManager = new Mock<IEnvironmentWindowsManager>();
        _windowsManager.SetupGet(x => x.Windows).Returns(new List<EnvironmentWindow>());
        _outputArea = new Mock<IOutputArea>();
        _view = new Mock<IEnvironmentWindowView>();
        _interpreter = new Mock<IInterpreter>();
        _interpreterFactory = new Mock<IInterpreterFactory>();
        _interpreterFactory.Setup(x => x.Make(_project, _outputArea.Object)).Returns(_interpreter.Object);
        _command = new RunProgramCommand(_interpreterFactory.Object, _selectedProject.Object, _windowsManager.Object, _outputArea.Object);
    }

    [Test]
    public void ProjectNotSelected_NoRun()
    {
        _selectedProject.SetupGet(x => x.Project).Returns((Project)null);
        _command.Execute(null);
        _interpreter.Verify(x => x.Run(), Times.Never());
    }

    [Test]
    public void ProjectSelected_Run()
    {
        _command.Execute(null);
        _interpreter.Verify(x => x.Run(), Times.Once());
    }

    [Test]
    public void SaveOpenedEditorsBeforeRun()
    {
        var model1 = new Mock<IEnvironmentWindowModel>();
        var model2 = new Mock<IEnvironmentWindowModel>();
        var model3 = new Mock<IEnvironmentWindowModel>();
        var saveable1 = model1.As<ISaveableEnvironmentWindow>();
        var saveable2 = model2.As<ISaveableEnvironmentWindow>();
        var saveable3 = model3.As<ISaveableEnvironmentWindow>();
        var windows = new List<EnvironmentWindow>
        {
            new("1", model1.Object, _view.Object),
            new("1", model2.Object, _view.Object),
            new("1", model3.Object, _view.Object)
        };
        _windowsManager.SetupGet(x => x.Windows).Returns(windows);

        _command.Execute(null);

        saveable1.Verify(x => x.Save(), Times.Once());
        saveable2.Verify(x => x.Save(), Times.Once());
        saveable3.Verify(x => x.Save(), Times.Once());
    }

    [Test]
    public void ClearOutputArea()
    {
        _command.Execute(null);
        _outputArea.Verify(x => x.Clear(), Times.Once());
    }
}
