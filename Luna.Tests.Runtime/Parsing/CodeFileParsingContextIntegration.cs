using System.Linq;
using Luna.Infrastructure;
using Luna.Parsing;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Parsing;

internal class CodeFileParsingContextIntegration
{
    private Mock<IFileSystem> _fileSystem;
    private CodeFileParsingContext _context;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();
    }

    [Test]
    public void ParseAll()
    {
        var codeFileText = @"
                // comment
                import 'file1.luna'
                const WIDTH 10
                const HEIGHT 20
                (func (x y) (1 2 3))";
        _fileSystem.Setup(x => x.ReadFileText("file2.luna")).Returns(codeFileText);
        var root = new DirectoryProjectItem("", null);
        var file1 = new CodeFileProjectItem("file1.luna", root, _fileSystem.Object);
        var file2 = new CodeFileProjectItem("file2.luna", root, _fileSystem.Object);

        _context = new CodeFileParsingContext(new[] { file1, file2 }, file2);
        _context.ParseImports();
        _context.ParseFunctions();

        Assert.AreEqual(1, file2.CodeModel.Imports.Count);
        Assert.AreEqual("file1.luna", file2.CodeModel.Imports.First().FilePath);
        Assert.AreEqual(2, file2.CodeModel.Constants.Count);
        Assert.AreEqual("WIDTH", file2.CodeModel.Constants.First().Name);
        Assert.AreEqual("HEIGHT", file2.CodeModel.Constants.Last().Name);
        Assert.AreEqual(1, file2.CodeModel.Functions.Count);
        Assert.AreEqual("func", file2.CodeModel.Functions.First().Name);
    }
}
