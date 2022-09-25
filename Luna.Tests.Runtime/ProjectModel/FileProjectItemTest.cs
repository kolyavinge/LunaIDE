using System.Linq;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.ProjectModel;

internal class FileProjectItemTest
{
    private Mock<IFileSystem> _fileSystem;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();
    }

    [Test]
    public void PathFromRoot()
    {
        var root = new DirectoryProjectItem(@"c:\path\project", null);

        var directory1 = new DirectoryProjectItem(@"c:\path\project\directory1", root);
        root.AddChild(directory1);

        var directory2 = new DirectoryProjectItem(@"c:\path\project\directory1\directory2", directory1);
        directory1.AddChild(directory2);

        var file = new CodeFileProjectItem(@"c:\path\project\directory1\directory2\codeFile", directory2, _fileSystem.Object);
        directory2.AddChild(file);

        Assert.AreEqual(@"directory1\directory2\codeFile", file.PathFromRoot);
    }

    [Test]
    public void AllParent()
    {
        var root = new DirectoryProjectItem(@"c:\path\project", null);

        var directory1 = new DirectoryProjectItem(@"c:\path\project\directory1", root);
        root.AddChild(directory1);

        var directory2 = new DirectoryProjectItem(@"c:\path\project\directory1\directory2", directory1);
        directory1.AddChild(directory2);

        var file = new CodeFileProjectItem(@"c:\path\project\directory1\directory2\codeFile", directory2, _fileSystem.Object);
        directory2.AddChild(file);

        var result = file.AllParents.ToList();
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("project", result[0].Name);
        Assert.AreEqual("directory1", result[1].Name);
        Assert.AreEqual("directory2", result[2].Name);
    }
}
