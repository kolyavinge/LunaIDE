using System;
using System.Collections.Generic;
using Luna.Infrastructure;
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
    }

    [Test]
    public void SetCodeFiles_UpdateRequest()
    {
        _updater.SetCodeFiles(_projectItems);

        _updater.OnTimerTick(_updater, EventArgs.Empty);

        _codeModelBuilder.Verify(x => x.BuildFor(_projectItems), Times.Once());
    }

    [Test]
    public void UpdateRequest()
    {
        _updater.SetCodeFiles(_projectItems);

        _updater.OnTimerTick(_updater, EventArgs.Empty);
        _updater.UpdateRequest();
        _updater.OnTimerTick(_updater, EventArgs.Empty);

        _codeModelBuilder.Verify(x => x.BuildFor(_projectItems), Times.Exactly(2));
    }

    [Test]
    public void NoUpdateRequest()
    {
        _updater.OnTimerTick(_updater, EventArgs.Empty);

        _codeModelBuilder.Verify(x => x.BuildFor(_projectItems), Times.Never());
    }
}
