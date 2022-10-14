﻿using System;
using Luna.IDE.CodeEditing;
using Luna.IDE.Common;
using Luna.IDE.ProjectChanging;
using Luna.IDE.Versioning;
using Luna.Infrastructure;
using Moq;
using NUnit.Framework;
using VersionControl.Core;

namespace Luna.Tests.IDE.ProjectChanging;

internal class ProjectChangesTest
{
    private Mock<IProjectRepository> _projectRepository;
    private Mock<ICodeEditorSaver> _codeEditorSaver;
    private Mock<ITimer> _timer;
    private Mock<ITimerManager> _timerManager;
    private ProjectChanges _projectChanges;

    [SetUp]
    public void Setup()
    {
        _projectRepository = new Mock<IProjectRepository>();
        _codeEditorSaver = new Mock<ICodeEditorSaver>();
        _timer = new Mock<ITimer>();
        _timerManager = new Mock<ITimerManager>();
        _projectChanges = new ProjectChanges(_projectRepository.Object, _codeEditorSaver.Object, _timerManager.Object);
    }

    [Test]
    public void Constructor()
    {
        Assert.False(_projectChanges.IsRepositoryExist);
        Assert.False(_projectChanges.IsCommitAllowed);
        Assert.That(_projectChanges.Comment, Is.EqualTo(""));
        Assert.That(_projectChanges.Included.Name, Is.EqualTo(""));
        Assert.That(_projectChanges.Excluded.Name, Is.EqualTo(""));
        Assert.That(_projectChanges.Included.Children, Has.Count.EqualTo(0));
        Assert.That(_projectChanges.Excluded.Children, Has.Count.EqualTo(0));
    }

    [Test]
    public void RepositoryInitialized_RaiseRepositoryOpened()
    {
        _projectRepository.Setup(x => x.Included).Returns(new VersionedDirectory("included"));
        _projectRepository.Setup(x => x.Excluded).Returns(new VersionedDirectory("excluded"));
        var repositoryOpenedFired = 0;
        _projectChanges.RepositoryOpened += (s, e) => repositoryOpenedFired++;

        _projectRepository.Raise(x => x.RepositoryInitialized += null, EventArgs.Empty);

        Assert.That(repositoryOpenedFired, Is.EqualTo(1));
    }

    [Test]
    public void RepositoryInitialized_NoActivated()
    {
        _projectRepository.Setup(x => x.Included).Returns(new VersionedDirectory("included"));
        _projectRepository.Setup(x => x.Excluded).Returns(new VersionedDirectory("excluded"));
        _projectRepository.SetupGet(x => x.IsRepositoryExist).Returns(true);
        _timerManager.Setup(x => x.CreateAndStart(TimeSpan.FromSeconds(2), It.IsAny<EventHandler>())).Returns(_timer.Object);

        _projectRepository.Raise(x => x.RepositoryInitialized += null, EventArgs.Empty);

        _timer.Verify(x => x.Stop(), Times.Never());
        _projectRepository.VerifyGet(x => x.IsRepositoryExist, Times.Once());
    }

    [Test]
    public void RepositoryInitialized_Activated()
    {
        _projectRepository.Setup(x => x.Included).Returns(new VersionedDirectory("included"));
        _projectRepository.Setup(x => x.Excluded).Returns(new VersionedDirectory("excluded"));
        _projectRepository.SetupGet(x => x.IsRepositoryExist).Returns(true);
        _timerManager.Setup(x => x.CreateAndStart(TimeSpan.FromSeconds(2), It.IsAny<EventHandler>())).Returns(_timer.Object);
        _projectChanges.Activate();

        _projectRepository.Raise(x => x.RepositoryInitialized += null, EventArgs.Empty);

        _timer.Verify(x => x.Stop(), Times.Once());
        _projectRepository.VerifyGet(x => x.IsRepositoryExist, Times.Exactly(3));
    }

    [Test]
    public void CreateRepository()
    {
        _projectChanges.CreateRepository();

        _projectRepository.Verify(x => x.OpenOrCreateRepository(), Times.Once());
    }

    [Test]
    public void RepositoryInitialized_IncludedExcluded()
    {
        var included = new VersionedDirectory("included");
        var excluded = new VersionedDirectory("excluded");
        _projectRepository.Setup(x => x.Included).Returns(included);
        _projectRepository.Setup(x => x.Excluded).Returns(excluded);

        _projectRepository.Raise(x => x.RepositoryInitialized += null, EventArgs.Empty);

        Assert.That(_projectChanges.Included.Name, Is.EqualTo("included"));
        Assert.That(_projectChanges.Excluded.Name, Is.EqualTo("excluded"));
    }

    [Test]
    public void Activate_RepositoryNotExist()
    {
        _projectRepository.SetupGet(x => x.IsRepositoryExist).Returns(false);
        var statusUpdatedFired = 0;
        _projectChanges.StatusUpdated += (s, e) => statusUpdatedFired++;

        _projectChanges.Activate();

        Assert.That(statusUpdatedFired, Is.EqualTo(0));
        _timerManager.Verify(x => x.CreateAndStart(TimeSpan.FromSeconds(2), It.IsAny<EventHandler>()), Times.Never());
    }

    [Test]
    public void Activate_RepositoryExist()
    {
        _projectRepository.SetupGet(x => x.IsRepositoryExist).Returns(true);
        var statusUpdatedFired = 0;
        _projectChanges.StatusUpdated += (s, e) => statusUpdatedFired++;

        _projectChanges.Activate();

        Assert.That(statusUpdatedFired, Is.EqualTo(1));
        _timerManager.Verify(x => x.CreateAndStart(TimeSpan.FromSeconds(2), It.IsAny<EventHandler>()), Times.Once());
    }

    [Test]
    public void OnTimer_UpdateStatus()
    {
        _projectRepository.SetupGet(x => x.IsRepositoryExist).Returns(true);
        var statusUpdatedFired = 0;
        _projectChanges.StatusUpdated += (s, e) => statusUpdatedFired++;
        Action<TimeSpan, EventHandler> callback = (ts, e) => e.Invoke(this, EventArgs.Empty);
        _timerManager.Setup(x => x.CreateAndStart(TimeSpan.FromSeconds(2), It.IsAny<EventHandler>())).Callback(callback);

        _projectChanges.Activate();

        Assert.That(statusUpdatedFired, Is.EqualTo(2));
        _codeEditorSaver.Verify(x => x.SaveOpenedEditors(), Times.Once());
    }

    [Test]
    public void Deactivate()
    {
        _projectRepository.SetupGet(x => x.IsRepositoryExist).Returns(true);
        _timerManager.Setup(x => x.CreateAndStart(TimeSpan.FromSeconds(2), It.IsAny<EventHandler>())).Returns(_timer.Object);
        _projectChanges.Activate();

        _projectChanges.Deactivate();

        _timer.Verify(x => x.Stop(), Times.Once());
    }

    [Test]
    public void Deactivate_TimerNull()
    {
        _projectRepository.SetupGet(x => x.IsRepositoryExist).Returns(true);
        _projectChanges.Activate();

        _projectChanges.Deactivate();
    }

    [Test]
    public void IncludeToCommit()
    {
        var file1 = new VersionedFile(1, "", "", 1, FileActionKind.Add);
        var file2 = new VersionedFile(2, "", "", 1, FileActionKind.Add);
        var items = new TreeItem[]
        {
            new VersionedFileTreeItem(null, file1),
            new VersionedFileTreeItem(null, file2)
        };

        _projectChanges.IncludeToCommit(items);

        _projectRepository.Verify(x => x.IncludeToCommit(new[] { file1, file2 }), Times.Once());
    }

    [Test]
    public void ExcludeFromCommit()
    {
        var file1 = new VersionedFile(1, "", "", 1, FileActionKind.Add);
        var file2 = new VersionedFile(2, "", "", 1, FileActionKind.Add);
        var items = new TreeItem[]
        {
            new VersionedFileTreeItem(null, file1),
            new VersionedFileTreeItem(null, file2)
        };

        _projectChanges.ExcludeFromCommit(items);

        _projectRepository.Verify(x => x.ExcludeFromCommit(new[] { file1, file2 }), Times.Once());
    }

    [Test]
    public void MakeCommit_SaveOpenedEditors()
    {
        _projectChanges.MakeCommit();

        _codeEditorSaver.Verify(x => x.SaveOpenedEditors(), Times.Once());
    }

    [Test]
    public void MakeCommit_ResetComment()
    {
        _projectRepository.Setup(x => x.Included).Returns(new VersionedDirectory(""));
        _projectChanges.Comment = "comment";

        _projectChanges.MakeCommit();

        Assert.That(_projectChanges.Comment, Is.EqualTo(""));
        Assert.False(_projectChanges.IsCommitAllowed);
    }

    [Test]
    public void MakeCommit_StatusUpdated()
    {
        var statusUpdatedFired = 0;
        _projectChanges.StatusUpdated += (s, e) => statusUpdatedFired++;

        _projectChanges.MakeCommit();

        Assert.That(statusUpdatedFired, Is.EqualTo(1));
        _projectRepository.Verify(x => x.UpdateStatus(), Times.Once());
    }

    [Test]
    public void IsCommitAllowed_IncludeAndComment()
    {
        var included = new VersionedDirectory("");
        included.AddFiles(new VersionedFile[] { new(1, "", "", 1, FileActionKind.Add) });
        _projectRepository.SetupGet(x => x.Included).Returns(included);

        _projectChanges.Comment = "comment";

        Assert.True(_projectChanges.IsCommitAllowed);
    }

    [Test]
    public void IsCommitAllowed_IncludeAndNoComment()
    {
        _projectChanges.Comment = "";

        Assert.False(_projectChanges.IsCommitAllowed);
    }

    [Test]
    public void IsCommitAllowed_NoIncludeAndComment()
    {
        var included = new VersionedDirectory("");
        _projectRepository.SetupGet(x => x.Included).Returns(included);

        _projectChanges.Comment = "comment";

        Assert.False(_projectChanges.IsCommitAllowed);
    }
}