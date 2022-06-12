using System.Linq;
using Luna.IDE.Model;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.Model;

public class ProjectTreeItemTest
{
    private Mock<IFileSystem> _fileSystem;
    private DirectoryProjectItem _rootDirectory;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();
        _rootDirectory = new DirectoryProjectItem("root", null);
        var directory = new DirectoryProjectItem("root\\directory", _rootDirectory);
        _rootDirectory.AddChild(directory);
        var file = new CodeFileProjectItem("root\\file", directory, _fileSystem.Object);
        directory.AddChild(file);
    }

    [Test]
    public void Depth()
    {
        var treeItem = new ProjectTreeItem(_rootDirectory);
        Assert.AreEqual(0, treeItem.Depth);
        Assert.AreEqual(0, treeItem.Children.First().Depth);
        Assert.AreEqual(1, treeItem.Children.First().Children.First().Depth);
    }

    [Test]
    public void CollapseWithSelectedChildren()
    {
        var treeItem = new ProjectTreeItem(_rootDirectory) { IsExpanded = true };
        treeItem.Children.First().IsSelected = true;
        treeItem.IsExpanded = false;
        Assert.True(treeItem.IsSelected);
    }
}
