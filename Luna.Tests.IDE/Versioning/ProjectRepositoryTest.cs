using System;
using System.Linq;
using Luna.IDE.ProjectExploration;
using Luna.IDE.Versioning;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;
using VersionControl.Core;

namespace Luna.Tests.IDE.Versioning;

internal class ProjectRepositoryTest
{
    private Mock<IFileSystem> _fileSystem;
    private string _projectPath;
    private Project _project;
    private Mock<IVersionControlRepository> _versionControlRepository;
    private Mock<IVersionControlRepositoryFactory> _repositoryFactory;
    private Mock<IProjectLoader> _projectLoader;
    private ProjectRepository _repository;

    [SetUp]
    public void Setup()
    {
        _fileSystem = new Mock<IFileSystem>();
        _projectPath = "c:\\project path";
        _project = new Project(_projectPath, _fileSystem.Object);
        _fileSystem.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(new string[0]);
        _fileSystem.Setup(x => x.GetFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(new string[0]);
        _versionControlRepository = new Mock<IVersionControlRepository>();
        _repositoryFactory = new Mock<IVersionControlRepositoryFactory>();
        _projectLoader = new Mock<IProjectLoader>();
        _repository = new ProjectRepository(_repositoryFactory.Object, _projectLoader.Object);
    }

    [Test]
    public void Constructor_EmptyIncludedExcluded()
    {
        Assert.That(_repository.Included.Name, Is.EqualTo(""));
        Assert.That(_repository.Excluded.Name, Is.EqualTo(""));
        Assert.That(_repository.Included.InnerDirectories.Count, Is.EqualTo(0));
        Assert.That(_repository.Included.InnerFiles.Count, Is.EqualTo(0));
        Assert.That(_repository.Excluded.InnerDirectories.Count, Is.EqualTo(0));
        Assert.That(_repository.Excluded.InnerFiles.Count, Is.EqualTo(0));
    }

    [Test]
    public void Constructor_Status()
    {
        Assert.That(_repository.Status, Is.EqualTo(VersionedStatus.Empty));
        _repositoryFactory.Verify(x => x.GetDummyRepository(), Times.Once());
    }

    [Test]
    public void IsRepositoryExist()
    {
        Assert.False(_repository.IsRepositoryExist);

        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        _repositoryFactory.Setup(x => x.IsRepositoryExist(_projectPath)).Returns(false);
        Assert.False(_repository.IsRepositoryExist);
        _repositoryFactory.Verify(x => x.IsRepositoryExist(_projectPath), Times.Exactly(1));

        _repositoryFactory.Setup(x => x.IsRepositoryExist(_projectPath)).Returns(true);
        Assert.True(_repository.IsRepositoryExist);
        _repositoryFactory.Verify(x => x.IsRepositoryExist(_projectPath), Times.Exactly(2));
    }

    [Test]
    public void ProjectOpen_NoRepository_NoRepositoryInitialized()
    {
        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        _repositoryFactory.Setup(x => x.IsRepositoryExist(_projectPath)).Returns(false);

        _projectLoader.Raise(x => x.ProjectOpened += null, new ProjectOpenedEventArgs(null));
    }

    [Test]
    public void ProjectOpen_RepositoryNotExists()
    {
        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        _repositoryFactory.Setup(x => x.IsRepositoryExist(_projectPath)).Returns(false);
        var repositoryInitializedFired = 0;
        _repository.RepositoryInitialized += (s, e) => repositoryInitializedFired++;

        _projectLoader.Raise(x => x.ProjectOpened += null, new ProjectOpenedEventArgs(null));

        Assert.That(repositoryInitializedFired, Is.EqualTo(1));
        Assert.That(_repository.Status, Is.EqualTo(VersionedStatus.Empty));
        _repositoryFactory.Verify(x => x.IsRepositoryExist(_projectPath), Times.Once());
        _repositoryFactory.Verify(x => x.GetDummyRepository(), Times.Exactly(2));
    }

    [Test]
    public void ProjectOpen_RepositoryExists()
    {
        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        _repositoryFactory.Setup(x => x.IsRepositoryExist(_projectPath)).Returns(true);
        var repositoryInitializedFired = 0;
        _repository.RepositoryInitialized += (s, e) => repositoryInitializedFired++;

        _projectLoader.Raise(x => x.ProjectOpened += null, new ProjectOpenedEventArgs(null));

        Assert.That(repositoryInitializedFired, Is.EqualTo(1));
        Assert.That(_repository.Status, Is.EqualTo(VersionedStatus.Empty));
        Assert.That(_repository.Included.Name, Is.EqualTo("project path"));
        Assert.That(_repository.Excluded.Name, Is.EqualTo("project path"));
        _repositoryFactory.Verify(x => x.IsRepositoryExist(_projectPath), Times.Once());
        _repositoryFactory.Verify(x => x.OpenRepository(_projectPath), Times.Once());
    }

    [Test]
    public void ProjectOpen_RepositoryExists_NoRepositoryInitialized()
    {
        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        _repositoryFactory.Setup(x => x.IsRepositoryExist(_projectPath)).Returns(true);

        _projectLoader.Raise(x => x.ProjectOpened += null, new ProjectOpenedEventArgs(null));
    }

    [Test]
    public void UpdateStatus_NoDifference()
    {
        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        _versionControlRepository.Setup(x => x.GetStatus()).Returns(VersionedStatus.Empty);
        _repositoryFactory.Setup(x => x.OpenRepository(_projectPath)).Returns(_versionControlRepository.Object);
        _repository.OpenOrCreateRepository();

        _repository.UpdateStatus();

        Assert.That(_repository.Included.InnerDirectories.Count, Is.EqualTo(0));
        Assert.That(_repository.Included.InnerFiles.Count, Is.EqualTo(0));
        Assert.That(_repository.Excluded.InnerDirectories.Count, Is.EqualTo(0));
        Assert.That(_repository.Excluded.InnerFiles.Count, Is.EqualTo(0));
    }

    [Test]
    public void UpdateStatus_AddFile()
    {
        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        var status = new VersionedStatus(new VersionControl.Core.VersionedFile[] { new(10, "file1", "file1", 1, FileActionKind.Add) });
        _versionControlRepository.Setup(x => x.GetStatus()).Returns(status);
        _repositoryFactory.Setup(x => x.OpenRepository(_projectPath)).Returns(_versionControlRepository.Object);
        _repository.OpenOrCreateRepository();

        _repository.UpdateStatus();

        Assert.That(_repository.Status, Is.EqualTo(status));
        Assert.That(_repository.Excluded.InnerFiles.Count, Is.EqualTo(1));
        Assert.That(_repository.Excluded.InnerFiles.First().FullPath, Is.EqualTo("file1"));
        Assert.That(_repository.Included.InnerFiles.Count, Is.EqualTo(0));
    }

    [Test]
    public void UpdateStatus_DeleteFileFromExcluded()
    {
        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        var status = new VersionedStatus(new VersionControl.Core.VersionedFile[] { new(10, "file1", "file1", 1, FileActionKind.Add) });
        _versionControlRepository.Setup(x => x.GetStatus()).Returns(status);
        _repositoryFactory.Setup(x => x.OpenRepository(_projectPath)).Returns(_versionControlRepository.Object);
        _repository.OpenOrCreateRepository();
        _repository.UpdateStatus();
        status = new VersionedStatus(new VersionControl.Core.VersionedFile[0]);
        _versionControlRepository.Setup(x => x.GetStatus()).Returns(status);

        Assert.That(_repository.Excluded.InnerFiles.Count, Is.EqualTo(1));

        _repository.UpdateStatus();

        Assert.That(_repository.Excluded.InnerFiles.Count, Is.EqualTo(0));
    }

    [Test]
    public void UpdateStatus_DeleteFileFromIncluded()
    {
        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        var files = new Luna.IDE.Versioning.VersionedFile[] { new(_versionControlRepository.Object, new(10, "file1", "file1", 1, FileActionKind.Add)) };
        var status = new VersionedStatus(files.Select(x => x.InnerVersionedFile).ToList());
        _versionControlRepository.Setup(x => x.GetStatus()).Returns(status);
        _repositoryFactory.Setup(x => x.OpenRepository(_projectPath)).Returns(_versionControlRepository.Object);
        _repository.OpenOrCreateRepository();
        _repository.UpdateStatus();
        status = new VersionedStatus(new VersionControl.Core.VersionedFile[0]);
        _versionControlRepository.Setup(x => x.GetStatus()).Returns(status);

        _repository.Excluded.DeleteFiles(files);
        _repository.Included.AddFiles(files);

        _repository.UpdateStatus();

        Assert.That(_repository.Excluded.InnerFiles.Count, Is.EqualTo(0));
    }

    [Test]
    public void IncludeToCommit()
    {
        var files = new Luna.IDE.Versioning.VersionedFile[] { new(_versionControlRepository.Object, new(10, "file1", "file1", 1, FileActionKind.Add)) };
        _repository.Excluded.AddFiles(files);

        Assert.That(_repository.Included.InnerFiles.Count, Is.EqualTo(0));
        Assert.That(_repository.Excluded.InnerFiles.Count, Is.EqualTo(1));

        _repository.IncludeToCommit(files);

        Assert.That(_repository.Included.InnerFiles.Count, Is.EqualTo(1));
        Assert.That(_repository.Excluded.InnerFiles.Count, Is.EqualTo(0));
    }

    [Test]
    public void ExcludeFromCommit()
    {
        var files = new Luna.IDE.Versioning.VersionedFile[] { new(_versionControlRepository.Object, new(10, "file1", "file1", 1, FileActionKind.Add)) };
        _repository.Included.AddFiles(files);

        Assert.That(_repository.Included.InnerFiles.Count, Is.EqualTo(1));
        Assert.That(_repository.Excluded.InnerFiles.Count, Is.EqualTo(0));

        _repository.ExcludeFromCommit(files);

        Assert.That(_repository.Included.InnerFiles.Count, Is.EqualTo(0));
        Assert.That(_repository.Excluded.InnerFiles.Count, Is.EqualTo(1));
    }

    [Test]
    public void MakeCommit_RaiseCommitMade()
    {
        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        var versionedFile = new VersionControl.Core.VersionedFile(10, "file1", "file1", 1, FileActionKind.Add);
        var filesToCommit = new Luna.IDE.Versioning.VersionedFile[] { new(_versionControlRepository.Object, versionedFile) };
        _versionControlRepository.Setup(x => x.MakeCommit("comment", new[] { versionedFile })).Returns(new CommitResult(123));
        _repositoryFactory.Setup(x => x.OpenRepository(_projectPath)).Returns(_versionControlRepository.Object);
        _repository.OpenOrCreateRepository();
        _repository.Included.AddFiles(filesToCommit);
        var commitMadeFired = 0;
        _repository.CommitMade += (s, e) => commitMadeFired++;

        var result = _repository.MakeCommit("comment");

        Assert.That(result.CommitId, Is.EqualTo(123));
        Assert.That(commitMadeFired, Is.EqualTo(1));
    }

    [Test]
    public void MakeCommit_NoRaiseCommitMade()
    {
        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        _repositoryFactory.Setup(x => x.OpenRepository(_projectPath)).Returns(_versionControlRepository.Object);
        _repository.OpenOrCreateRepository();

        _repository.MakeCommit("comment");
    }

    [Test]
    public void FindCommits()
    {
        var filter = new FindCommitsFilter();
        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        _versionControlRepository.Setup(x => x.FindCommits(filter)).Returns(new VersionControl.Core.Commit[] { new(123, "author", "comment", DateTime.Now) });
        _repositoryFactory.Setup(x => x.OpenRepository(_projectPath)).Returns(_versionControlRepository.Object);
        _repository.OpenOrCreateRepository();

        var result = _repository.FindCommits(filter).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(123));
    }

    [Test]
    public void UndoChanges_RaiseChangesUndone()
    {
        var versionedFile = new VersionControl.Core.VersionedFile(1, "", "", 10, FileActionKind.Add);
        var files = new Luna.IDE.Versioning.VersionedFile[] { new(_versionControlRepository.Object, versionedFile) };
        var changesUndoneFired = 0;
        _repository.ChangesUndone += (s, e) => changesUndoneFired++;
        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        _repositoryFactory.Setup(x => x.OpenRepository(_projectPath)).Returns(_versionControlRepository.Object);
        _repository.OpenOrCreateRepository();

        _repository.UndoChanges(files);

        Assert.That(changesUndoneFired, Is.EqualTo(1));
        _versionControlRepository.Verify(x => x.UndoChanges(new[] { versionedFile }), Times.Once());
    }

    [Test]
    public void UndoChanges_NotRaiseChangesUndone()
    {
        _projectLoader.SetupGet(x => x.Project).Returns(_project);
        _repositoryFactory.Setup(x => x.OpenRepository(_projectPath)).Returns(_versionControlRepository.Object);
        _repository.OpenOrCreateRepository();

        _repository.UndoChanges(new Luna.IDE.Versioning.VersionedFile[0]);
    }
}
