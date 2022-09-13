using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Infrastructure;
using Luna.Parsing;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.ProjectModel;

internal class CodeModelUpdaterTest
{
    private Mock<IFileSystem> _fileSystem;
    private List<CodeFileProjectItem> _projectItems;
    private Mock<ITimerManager> _timerManager;
    private Mock<ICodeModelBuilder> _codeModelBuilder;
    private CodeModelUpdater _updater;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();
        _projectItems = new();
        _projectItems.Add(new CodeFileProjectItem("", null, _fileSystem.Object));
        _timerManager = new Mock<ITimerManager>();
        _codeModelBuilder = new Mock<ICodeModelBuilder>();
        _updater = new CodeModelUpdater(_timerManager.Object, _codeModelBuilder.Object);
        _updater.SetCodeFiles(_projectItems);
    }

    [Test]
    public void UpdateCodeModelFromBuilder()
    {
        var newModel = new CodeModel();
        newModel.AddFunctionDeclaration(new("func", Enumerable.Empty<FunctionArgument>(), new()));
        _codeModelBuilder.Setup(x => x.BuildFor(_projectItems)).Callback(() => _projectItems.First().CodeModel = newModel);
        CodeModel callbackModel = null;
        CodeModelScopeIdentificatorsDifferent callbackDiff = null;
        _updater.Attach(_projectItems.First(), r => { callbackModel = r.NewModel; callbackDiff = r.Different; });

        _updater.OnTimerTick(_updater, EventArgs.Empty);

        Assert.NotNull(callbackModel);
        Assert.NotNull(callbackDiff);
        Assert.AreEqual(newModel, callbackModel);
        Assert.AreEqual(1, callbackDiff.AddedDeclaredFunctions.Count);
    }

    [Test]
    public void UpdateCodeModelFromExternal()
    {
        var newModel = new CodeModel();
        newModel.AddFunctionDeclaration(new("func", Enumerable.Empty<FunctionArgument>(), new()));
        CodeModel callbackModel = null;
        CodeModelScopeIdentificatorsDifferent callbackDiff = null;
        _updater.Attach(_projectItems.First(), r => { callbackModel = r.NewModel; callbackDiff = r.Different; });
        _projectItems.First().CodeModel = newModel;

        _updater.OnTimerTick(_updater, EventArgs.Empty);

        Assert.NotNull(callbackModel);
        Assert.NotNull(callbackDiff);
        Assert.AreEqual(newModel, callbackModel);
        Assert.AreEqual(1, callbackDiff.AddedDeclaredFunctions.Count);
    }

    [Test]
    public void AttachDetach()
    {
        _updater.Attach(_projectItems.First(), _ => { });
        _updater.Detach(_projectItems.First());
        _updater.Attach(_projectItems.First(), _ => { });
    }
}
