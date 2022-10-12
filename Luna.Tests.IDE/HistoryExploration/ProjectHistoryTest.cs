using System;
using System.Linq;
using Luna.IDE.HistoryExploration;
using Luna.IDE.ProjectExploration;
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
    private ProjectHistory _projectHistory;

    [SetUp]
    public void Setup()
    {
        _commit = new Mock<ICommit>();
        _commit.SetupGet(x => x.Details).Returns(new CommitedDirectory("root"));
        _projectLoader = new Mock<IProjectLoader>();
        _projectRepository = new Mock<IProjectRepository>();
        _projectHistory = new ProjectHistory(_projectLoader.Object, _projectRepository.Object);
    }

    [Test]
    public void Empty()
    {
        _projectHistory.Header.ToString();
        _projectHistory.Save();
        _projectHistory.Close();
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

        _projectLoader.Raise(x => x.ProjectOpened += null, EventArgs.Empty);

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
}
