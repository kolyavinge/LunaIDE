using System.Linq;
using Luna.Infrastructure;
using Luna.Parsing;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Parsing;

internal class CodeFileOrderLogicTest
{
    private Mock<IFileSystem> _fileSystem;
    private CodeFileOrderLogic _logic;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();
        _logic = new CodeFileOrderLogic();
    }

    [Test]
    public void ByImports_NoImports()
    {
        var item1 = new CodeFileProjectItem("", null, _fileSystem.Object);
        var item2 = new CodeFileProjectItem("", null, _fileSystem.Object);
        var item3 = new CodeFileProjectItem("", null, _fileSystem.Object);

        var result = _logic.ByImports(new[] { item1, item2, item3 }).ToList();

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(item1, result[0]);
        Assert.AreEqual(item2, result[1]);
        Assert.AreEqual(item3, result[2]);
    }

    [Test]
    public void ByImports_OneImport()
    {
        var child = new CodeFileProjectItem("", null, _fileSystem.Object);
        var parent = new CodeFileProjectItem("", null, _fileSystem.Object);
        parent.CodeModel.AddImportDirective(new ImportDirective("", child, 0, 0));

        var result = _logic.ByImports(new[] { parent, child }).ToList();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(child, result[0]);
        Assert.AreEqual(parent, result[1]);
    }

    [Test]
    public void ByImports_OneFileImportedInTwo()
    {
        var child = new CodeFileProjectItem("", null, _fileSystem.Object);
        var parent1 = new CodeFileProjectItem("", null, _fileSystem.Object);
        var parent2 = new CodeFileProjectItem("", null, _fileSystem.Object);
        parent1.CodeModel.AddImportDirective(new ImportDirective("", child, 0, 0));
        parent2.CodeModel.AddImportDirective(new ImportDirective("", child, 0, 0));

        var result = _logic.ByImports(new[] { parent1, parent2, child }).ToList();

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(child, result[0]);
        Assert.AreEqual(parent1, result[1]);
        Assert.AreEqual(parent2, result[2]);
    }

    [Test]
    public void ByImports_Recursive()
    {
        var item1 = new CodeFileProjectItem("", null, _fileSystem.Object);
        var item2 = new CodeFileProjectItem("", null, _fileSystem.Object);
        item1.CodeModel.AddImportDirective(new ImportDirective("", item2, 0, 0));
        item2.CodeModel.AddImportDirective(new ImportDirective("", item1, 0, 0));

        var result = _logic.ByImports(new[] { item1, item2 }).ToList();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(item2, result[0]);
        Assert.AreEqual(item1, result[1]);
    }
}
