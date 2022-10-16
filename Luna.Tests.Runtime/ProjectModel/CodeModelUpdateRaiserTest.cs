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
        _projectItems = new List<CodeFileProjectItem>();
        _projectItems.Add(new CodeFileProjectItem("", null, _fileSystem.Object));
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
    public void WithDiff()
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
}
