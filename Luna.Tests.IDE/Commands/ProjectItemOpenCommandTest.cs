using System.Linq;
using Luna.IDE.Commands;
using Luna.IDE.Model;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.Commands
{
    public class ProjectItemOpenCommandTest
    {
        private Mock<IFileSystem> _fileSystem;
        private Mock<IEnvironmentWindowsManager> _windowsManager;
        private Mock<IProjectItemEditorFactory> _editorFactory;
        private ProjectItemOpenCommand _command;

        [SetUp]
        public void Setup()
        {
            _fileSystem = new Mock<IFileSystem>();
            _windowsManager = new Mock<IEnvironmentWindowsManager>();
            _editorFactory = new Mock<IProjectItemEditorFactory>();
            _command = new ProjectItemOpenCommand(_windowsManager.Object, _editorFactory.Object);
        }

        [Test]
        public void EmptyList()
        {
            _command.Execute(Enumerable.Empty<ProjectItem>());
        }

        [Test]
        public void OneCodeFile()
        {
            var item = new CodeFileProjectItem("", null, _fileSystem.Object);
            var components = new EnvironmentWindowComponents(new Mock<IEnvironmentWindowModel>().Object, new object());
            _editorFactory.Setup(x => x.MakeEditorFor(item)).Returns(components);
            _windowsManager.Setup(x => x.FindWindowById(item)).Returns((EnvironmentWindow)null);
            var environmentWindow = new EnvironmentWindow(item, components.Model, components.View);
            _windowsManager.Setup(x => x.OpenWindow(item, components.Model, components.View)).Returns(environmentWindow);

            _command.Execute(new[] { item });

            _windowsManager.Verify(x => x.FindWindowById(item), Times.Once());
            _editorFactory.Verify(x => x.MakeEditorFor(item), Times.Once());
            _windowsManager.Verify(x => x.OpenWindow(item, components.Model, components.View), Times.Once());
            _windowsManager.Verify(x => x.ActivateWindow(environmentWindow), Times.Once());
        }

        [Test]
        public void TwoCodeFiles_FirstActivated()
        {
            var item1 = new CodeFileProjectItem("", null, _fileSystem.Object);
            var components1 = new EnvironmentWindowComponents(new Mock<IEnvironmentWindowModel>().Object, new object());
            _editorFactory.Setup(x => x.MakeEditorFor(item1)).Returns(components1);
            _windowsManager.Setup(x => x.FindWindowById(item1)).Returns((EnvironmentWindow)null);
            var environmentWindow1 = new EnvironmentWindow(item1, components1.Model, components1.View);
            _windowsManager.Setup(x => x.OpenWindow(item1, components1.Model, components1.View)).Returns(environmentWindow1);

            var item2 = new CodeFileProjectItem("", null, _fileSystem.Object);
            var components2 = new EnvironmentWindowComponents(new Mock<IEnvironmentWindowModel>().Object, new object());
            _editorFactory.Setup(x => x.MakeEditorFor(item2)).Returns(components2);
            _windowsManager.Setup(x => x.FindWindowById(item2)).Returns((EnvironmentWindow)null);
            var environmentWindow2 = new EnvironmentWindow(item2, components2.Model, components2.View);
            _windowsManager.Setup(x => x.OpenWindow(item2, components2.Model, components2.View)).Returns(environmentWindow2);

            _command.Execute(new[] { item1, item2 });

            _windowsManager.Verify(x => x.FindWindowById(item1), Times.Once());
            _editorFactory.Verify(x => x.MakeEditorFor(item1), Times.Once());
            _windowsManager.Verify(x => x.OpenWindow(item1, components1.Model, components1.View), Times.Once());
            _windowsManager.Verify(x => x.ActivateWindow(environmentWindow1), Times.Once());

            _windowsManager.Verify(x => x.FindWindowById(item2), Times.Once());
            _editorFactory.Verify(x => x.MakeEditorFor(item2), Times.Once());
            _windowsManager.Verify(x => x.OpenWindow(item2, components2.Model, components2.View), Times.Once());
            _windowsManager.Verify(x => x.ActivateWindow(environmentWindow2), Times.Never());
        }
    }
}
