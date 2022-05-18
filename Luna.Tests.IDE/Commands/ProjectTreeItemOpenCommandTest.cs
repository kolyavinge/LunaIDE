using System.Linq;
using Luna.IDE.Commands;
using Luna.IDE.Model;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.Commands;

public class ProjectTreeItemOpenCommandTest
{
    private ProjectTreeItem _rootTreeItem;
    private Mock<IProjectItemOpenCommand> _projectItemOpenCommand;
    private ProjectTreeItemOpenCommand _command;

    [SetUp]
    public void Setup()
    {
        var fileSystem = new Mock<IFileSystem>();

        var root = new DirectoryProjectItem("root", null);
        root.AddChild(new DirectoryProjectItem("directory 1", root));
        root.AddChild(new DirectoryProjectItem("directory 2", root));
        root.AddChild(new CodeFileProjectItem("file 1", root, fileSystem.Object));
        root.AddChild(new CodeFileProjectItem("file 2", root, fileSystem.Object));
        _rootTreeItem = new ProjectTreeItem(root);

        _projectItemOpenCommand = new Mock<IProjectItemOpenCommand>();
        _command = new ProjectTreeItemOpenCommand(_projectItemOpenCommand.Object);
    }

    [Test]
    public void EmptyItemList()
    {
        _command.Execute(Enumerable.Empty<ProjectTreeItem>());
    }

    [Test]
    public void ProjectItem_Ignore()
    {
        _command.Execute(new[] { _rootTreeItem });
        _projectItemOpenCommand.Verify(x => x.Execute(It.IsAny<object>()), Times.Never());
    }

    [Test]
    public void ProjectItemAndDirectory()
    {
        var directory1 = _rootTreeItem.Children.First();
        _command.Execute(new[] { _rootTreeItem, directory1 });
        Assert.AreEqual(true, directory1.IsExpanded);
        _projectItemOpenCommand.Verify(x => x.Execute(It.IsAny<object>()), Times.Never());
    }

    [Test]
    public void OneDirectorySelected_Expanded()
    {
        var directory1 = _rootTreeItem.Children.First();
        Assert.AreEqual(false, directory1.IsExpanded);
        _command.Execute(new[] { directory1 });
        Assert.AreEqual(true, directory1.IsExpanded);
        _command.Execute(new[] { directory1 });
        Assert.AreEqual(false, directory1.IsExpanded);
    }

    [Test]
    public void TwoDirectoriesSelected_NoExpand()
    {
        var directory1 = _rootTreeItem.Children.ToArray()[0];
        var directory2 = _rootTreeItem.Children.ToArray()[1];
        Assert.AreEqual(false, directory1.IsExpanded);
        Assert.AreEqual(false, directory2.IsExpanded);
        _command.Execute(new[] { directory1, directory2 });
        Assert.AreEqual(false, directory1.IsExpanded);
        Assert.AreEqual(false, directory2.IsExpanded);
    }

    [Test]
    public void TwoProjectItemsSelected_Open()
    {
        var file1 = _rootTreeItem.Children.ToArray()[2];
        var file2 = _rootTreeItem.Children.ToArray()[3];
        _command.Execute(new[] { file1, file2 });
        _projectItemOpenCommand.Verify(x => x.Execute(new[] { file1.ProjecItem, file2.ProjecItem }), Times.Once());
    }

    [Test]
    public void AllSelected_Filespen()
    {
        var directory1 = _rootTreeItem.Children.ToArray()[0];
        var directory2 = _rootTreeItem.Children.ToArray()[1];
        var file1 = _rootTreeItem.Children.ToArray()[2];
        var file2 = _rootTreeItem.Children.ToArray()[3];
        _command.Execute(new[] { directory1, directory2, file1, file2 });
        _projectItemOpenCommand.Verify(x => x.Execute(new[] { file1.ProjecItem, file2.ProjecItem }), Times.Once());
    }
}
