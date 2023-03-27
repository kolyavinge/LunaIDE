using System.Collections.Generic;
using System.Linq;
using Luna.CodeElements;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.ProjectModel;

internal class CodeModelUpdateRaiserTest
{
    private Mock<IFileSystem> _fileSystem;
    private List<CodeFileProjectItem> _projectItems;
    private CodeModelUpdateRaiser _raiser;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();
        _projectItems = new List<CodeFileProjectItem>
        {
            new("file1", null, _fileSystem.Object),
            new("file2", null, _fileSystem.Object)
        };
        _raiser = new CodeModelUpdateRaiser();
    }

    [Test]
    public void NoDiff()
    {
        _raiser.StoreOldCodeModels(_projectItems);
        CodeModel callbackOldCodeModel = null;
        CodeModelScopeIdentificatorsDifferent callbackDiff = null;
        _projectItems.First().CodeModelUpdated += (s, e) => { callbackOldCodeModel = e.OldCodeModel; callbackDiff = e.Different; };

        _raiser.RaiseUpdateCodeModelWithDiff();

        Assert.That(callbackOldCodeModel, Is.EqualTo(_projectItems.First().CodeModel));
        Assert.NotNull(callbackDiff);
        Assert.False(callbackDiff.AnyChanges);
    }

    [Test]
    public void Diff()
    {
        _raiser.StoreOldCodeModels(_projectItems);
        var oldCodeModel = _projectItems.First().CodeModel;
        var newCodeModel = new CodeModel();
        newCodeModel.AddFunctionDeclaration(new("func", Enumerable.Empty<FunctionArgument>(), new()));
        CodeModel callbackOldCodeModel = null;
        CodeModelScopeIdentificatorsDifferent callbackDiff = null;
        _projectItems.First().CodeModelUpdated += (s, e) => { callbackOldCodeModel = e.OldCodeModel; callbackDiff = e.Different; };
        _projectItems.First().CodeModel = newCodeModel;

        _raiser.RaiseUpdateCodeModelWithDiff();

        Assert.That(callbackOldCodeModel, Is.EqualTo(oldCodeModel));
        Assert.NotNull(callbackDiff);
        Assert.True(callbackDiff.AnyChanges);
        Assert.AreEqual(1, callbackDiff.AddedDeclaredFunctions.Count);
    }

    [Test]
    public void DiffWithImported()
    {
        _raiser.StoreOldCodeModels(_projectItems);
        _projectItems.First().CodeModel = new CodeModel();
        _projectItems.Last().CodeModel = new CodeModel();
        _projectItems.First().CodeModel.AddFunctionDeclaration(new("func", Enumerable.Empty<FunctionArgument>(), new()));
        _projectItems.Last().CodeModel.AddImportDirective(new("file1", _projectItems.First()));
        CodeModelScopeIdentificatorsDifferent callbackDiff = null;
        _projectItems.Last().CodeModelUpdated += (s, e) => { callbackDiff = e.Different; };

        _raiser.RaiseUpdateCodeModelWithDiff();

        Assert.NotNull(callbackDiff);
        Assert.True(callbackDiff.AnyChanges);
        Assert.AreEqual(1, callbackDiff.AddedImportedFunctions.Count);
    }
}
