using System;
using System.Linq;
using System.Text;
using Luna.IDE.HistoryExploration;
using Luna.IDE.ProjectExploration;
using Luna.IDE.TextDiff;
using Luna.IDE.Versioning;
using Moq;
using NUnit.Framework;
using VersionControl.Core;

namespace Luna.Tests.IDE.HistoryExploration;

internal class ProjectHistoryTest
{
    private Mock<ICommit> _commit;
    private Mock<IProjectLoader> _projectLoader;
    private Mock<IProjectRepository> _projectRepository;
    private Mock<ISingleTextDiff> _singleTextDiff;
    private ProjectHistory _projectHistory;

    [SetUp]
    public void Setup()
    {
        _commit = new Mock<ICommit>();
        _commit.SetupGet(x => x.Details).Returns(new CommitedDirectory("root"));
        _projectLoader = new Mock<IProjectLoader>();
        _projectRepository = new Mock<IProjectRepository>();
        _singleTextDiff = new Mock<ISingleTextDiff>();
        _projectHistory = new ProjectHistory(_projectLoader.Object, _projectRepository.Object, _singleTextDiff.Object);
    }

    [Test]
    public void Empty()
    {
        _projectHistory.Header.ToString();
    }

    [Test]
    public void SelectedCommit_DetailsRoot()
    {
        _projectHistory.SelectedCommit = _commit.Object;

        Assert.That(_projectHistory.DetailsRoot.Name, Is.EqualTo("root"));
    }

    [Test]
    public void OnProjectOpened_ResetSelectedCommit()
    {
        _projectHistory.SelectedCommit = _commit.Object;

        _projectLoader.Raise(x => x.ProjectOpened += null, new ProjectOpenedEventArgs(null));

        Assert.Null(_projectHistory.SelectedCommit);
    }

    [Test]
    public void OnCommitMade_NoSelectedCommit()
    {
        _projectHistory.SelectedCommit = null;

        _projectRepository.Raise(x => x.CommitMade += null, EventArgs.Empty);

        Assert.Null(_projectHistory.SelectedCommit);
    }

    [Test]
    public void OnCommitMade_WithSelectedCommit()
    {
        _projectRepository.Setup(x => x.FindCommits(It.IsAny<FindCommitsFilter>())).Returns(new[] { _commit.Object });
        _projectHistory.SelectedCommit = _commit.Object;
        _projectHistory.Commits.ToList();

        _projectRepository.Raise(x => x.CommitMade += null, EventArgs.Empty);

        Assert.That(_projectHistory.SelectedCommit, Is.EqualTo(_commit.Object));
    }

    [Test]
    public void AnyCommits()
    {
        _projectRepository.Setup(x => x.FindCommits(It.IsAny<FindCommitsFilter>())).Returns(new ICommit[0]);
        _projectHistory.Commits.ToList();
        Assert.False(_projectHistory.AnyCommits);

        _projectRepository.Setup(x => x.FindCommits(It.IsAny<FindCommitsFilter>())).Returns(new[] { _commit.Object });
        _projectHistory.Commits.ToList();
        Assert.True(_projectHistory.AnyCommits);
    }

    [Test]
    public void DetailsSelected()
    {
        var detail = new CommitDetail(1, FileActionKind.Add, 1, "file.ext");
        var versionControlRepository = new Mock<IVersionControlRepository>();
        versionControlRepository.Setup(x => x.GetFileContent(detail)).Returns(Encoding.UTF8.GetBytes("content"));
        versionControlRepository.Setup(x => x.GetFileContentBefore(detail)).Returns(Encoding.UTF8.GetBytes("content before"));
        var file = new CommitedFile(versionControlRepository.Object, detail);
        var root = new CommitedDirectory("root");
        root.AddFiles(new[] { file });
        _commit.SetupGet(x => x.Details).Returns(root);

        _projectHistory.SelectedCommit = _commit.Object;
        _projectHistory.DetailsRoot.Children.First().IsSelected = true;

        _singleTextDiff.Verify(x => x.MakeDiff(".ext", "content before", "content"), Times.Once());
    }

    [Test]
    public void DetailsSelected_False()
    {
        var detail = new CommitDetail(1, FileActionKind.Add, 1, "file.ext");
        var versionControlRepository = new Mock<IVersionControlRepository>();
        versionControlRepository.Setup(x => x.GetFileContent(detail)).Returns(Encoding.UTF8.GetBytes("content"));
        versionControlRepository.Setup(x => x.GetFileContentBefore(detail)).Returns(Encoding.UTF8.GetBytes("content before"));
        var file = new CommitedFile(versionControlRepository.Object, detail);
        var root = new CommitedDirectory("root");
        root.AddFiles(new[] { file });
        _commit.SetupGet(x => x.Details).Returns(root);

        _projectHistory.SelectedCommit = _commit.Object;
        _projectHistory.DetailsRoot.Children.First().IsSelected = false;

        _singleTextDiff.Verify(x => x.MakeDiff(".ext", "content before", "content"), Times.Never());
    }
}
