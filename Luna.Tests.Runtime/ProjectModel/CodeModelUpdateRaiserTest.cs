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
        CodeModelScopeIdentificatorsDifferent callbackDiff = null;
        _projectItems.First().CodeModelUpdated += (s, e) => callbackDiff = e.Different;

        _raiser.RaiseUpdateCodeModelWithDiff();

        Assert.NotNull(callbackDiff);
        Assert.False(callbackDiff.AnyChanges);
    }

    [Test]
    public void WithDiff()
    {
        _raiser.StoreOldCodeModels(_projectItems);
        var newModel = new CodeModel();
        newModel.AddFunctionDeclaration(new("func", Enumerable.Empty<FunctionArgument>(), new()));
        CodeModelScopeIdentificatorsDifferent callbackDiff = null;
        _projectItems.First().CodeModelUpdated += (s, e) => callbackDiff = e.Different;
        _projectItems.First().CodeModel = newModel;

        _raiser.RaiseUpdateCodeModelWithDiff();

        Assert.NotNull(callbackDiff);
        Assert.True(callbackDiff.AnyChanges);
        Assert.AreEqual(1, callbackDiff.AddedDeclaredFunctions.Count);
    }
}
