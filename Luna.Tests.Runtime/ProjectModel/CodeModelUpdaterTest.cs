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
    private readonly List<CodeFileProjectItem> _projectItems = new();
    private Mock<ITimerManager> _timerManager;
    private Mock<ICodeModelBuilder> _codeModelBuilder;
    private CodeModelUpdater _updater;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();
        _projectItems.Add(new CodeFileProjectItem("", null, _fileSystem.Object));
        _timerManager = new Mock<ITimerManager>();
        _codeModelBuilder = new Mock<ICodeModelBuilder>();
        _updater = new CodeModelUpdater(_timerManager.Object, _codeModelBuilder.Object);
        _updater.SetCodeFiles(_projectItems);
    }

    [Test]
    public void UpdateCodeModelFromBuilder()
    {
        var oldModel = _projectItems.First().CodeModel;
        var newModel = new CodeModel();
        _codeModelBuilder.Setup(x => x.BuildFor(_projectItems)).Callback(() => _projectItems.First().CodeModel = newModel);
        CodeModel callbackOldModel = null, callbackNewModel = null;
        _updater.Attach(_projectItems.First(), r => { callbackOldModel = r.OldModel; callbackNewModel = r.NewModel; });

        _updater.OnTimerTick(_updater, EventArgs.Empty);

        Assert.NotNull(callbackOldModel);
        Assert.NotNull(callbackNewModel);
        Assert.AreEqual(oldModel, callbackOldModel);
        Assert.AreEqual(newModel, callbackNewModel);
    }

    [Test]
    public void UpdateCodeModelFromExternal()
    {
        var oldModel = _projectItems.First().CodeModel;
        var newModel = new CodeModel();
        CodeModel callbackOldModel = null, callbackNewModel = null;
        _updater.Attach(_projectItems.First(), r => { callbackOldModel = r.OldModel; callbackNewModel = r.NewModel; });
        _projectItems.First().CodeModel = newModel;

        _updater.OnTimerTick(_updater, EventArgs.Empty);

        Assert.NotNull(callbackOldModel);
        Assert.NotNull(callbackNewModel);
        Assert.AreEqual(oldModel, callbackOldModel);
        Assert.AreEqual(newModel, callbackNewModel);
    }
}
