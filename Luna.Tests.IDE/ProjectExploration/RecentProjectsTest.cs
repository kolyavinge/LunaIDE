using System;
using System.Linq;
using Luna.IDE.Common;
using Luna.IDE.Configuration;
using Luna.IDE.ProjectExploration;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.ProjectExploration;

internal class RecentProjectsTest
{
    private DirectoryProjectItem _projectRoot;
    private Mock<IProject> _project;
    private Mock<IProjectLoader> _projectLoader;
    private Mock<IConfigStorage> _configStorage;
    private DateTime _lastAccess;
    private Mock<IDateTimeProvider> _dateTimeProvider;
    private RecentProjects _recentProjects;

    [SetUp]
    public void Setup()
    {
        _projectRoot = new DirectoryProjectItem("full path", null);
        _project = new Mock<IProject>();
        _project.SetupGet(x => x.Root).Returns(_projectRoot);
        _projectLoader = new Mock<IProjectLoader>();
        _configStorage = new Mock<IConfigStorage>();
        _lastAccess = new DateTime(2000, 1, 1);
        _dateTimeProvider = new Mock<IDateTimeProvider>();
        _dateTimeProvider.Setup(x => x.GetNowDateTime()).Returns(_lastAccess);
    }

    [Test]
    public void LoadProjectsOnStartup_NoProjects()
    {
        MakeRecentProjects();

        Assert.That(_recentProjects.Projects.Count(), Is.EqualTo(0));
        _configStorage.Verify(x => x.GetById<RecentProjectPoco>(0), Times.Once());
        _configStorage.Verify(x => x.GetById<RecentProjectPoco>(1), Times.Never());
    }

    [Test]
    public void LoadProjectsOnStartup_TwoProjects()
    {
        _configStorage.Setup(x => x.GetById<RecentProjectPoco>(0)).Returns(new RecentProjectPoco { Id = 0, ProjectFullPath = "project_0" });
        _configStorage.Setup(x => x.GetById<RecentProjectPoco>(1)).Returns(new RecentProjectPoco { Id = 1, ProjectFullPath = "project_1" });

        MakeRecentProjects();

        Assert.That(_recentProjects.Projects.Count(), Is.EqualTo(2));
        Assert.That(_recentProjects.Projects.ElementAt(0).FullPath, Is.EqualTo("project_0"));
        Assert.That(_recentProjects.Projects.ElementAt(1).FullPath, Is.EqualTo("project_1"));
        _configStorage.Verify(x => x.GetById<RecentProjectPoco>(0), Times.Once());
        _configStorage.Verify(x => x.GetById<RecentProjectPoco>(1), Times.Once());
        _configStorage.Verify(x => x.GetById<RecentProjectPoco>(2), Times.Once());
        _configStorage.Verify(x => x.GetById<RecentProjectPoco>(3), Times.Never());
    }

    [Test]
    public void LoadProjectsOnStartup_MaxRecentProjectsCount()
    {
        for (int i = 0; i < RecentProjects.MaxRecentProjectsCount; i++)
        {
            _configStorage.Setup(x => x.GetById<RecentProjectPoco>(i)).Returns(new RecentProjectPoco { Id = i, ProjectFullPath = $"project_{i}" });
        }

        MakeRecentProjects();

        Assert.That(_recentProjects.Projects.Count(), Is.EqualTo(RecentProjects.MaxRecentProjectsCount));
        for (int i = 0; i < RecentProjects.MaxRecentProjectsCount; i++)
        {
            _configStorage.Verify(x => x.GetById<RecentProjectPoco>(i), Times.Once());
        }
        _configStorage.Verify(x => x.GetById<RecentProjectPoco>(RecentProjects.MaxRecentProjectsCount + 1), Times.Never());
    }

    [Test]
    public void ProjectsOrderByName()
    {
        _configStorage.Setup(x => x.GetById<RecentProjectPoco>(0)).Returns(new RecentProjectPoco { Id = 1, ProjectFullPath = "path\\project_3" });
        _configStorage.Setup(x => x.GetById<RecentProjectPoco>(1)).Returns(new RecentProjectPoco { Id = 2, ProjectFullPath = "path\\project_2" });
        _configStorage.Setup(x => x.GetById<RecentProjectPoco>(2)).Returns(new RecentProjectPoco { Id = 3, ProjectFullPath = "path\\project_1" });

        MakeRecentProjects();

        Assert.That(_recentProjects.Projects.ElementAt(0).Name, Is.EqualTo("project_1"));
        Assert.That(_recentProjects.Projects.ElementAt(1).Name, Is.EqualTo("project_2"));
        Assert.That(_recentProjects.Projects.ElementAt(2).Name, Is.EqualTo("project_3"));
    }

    [Test]
    public void OpenProject_NewProject()
    {
        MakeRecentProjects();

        RaiseProjectOpened();

        Assert.That(_recentProjects.Projects.Count(), Is.EqualTo(1));
        Assert.That(_recentProjects.Projects.ElementAt(0).FullPath, Is.EqualTo("full path"));
        _dateTimeProvider.Verify(x => x.GetNowDateTime(), Times.Once());
        _configStorage.Verify(x => x.Save(new RecentProjectPoco { Id = 0, ProjectFullPath = "full path", LastAccess = _lastAccess }), Times.Once());
    }

    [Test]
    public void OpenProject_ExistProject()
    {
        MakeRecentProjects();

        RaiseProjectOpened();
        _dateTimeProvider.Setup(x => x.GetNowDateTime()).Returns(new DateTime(2000, 1, 2));
        RaiseProjectOpened();

        Assert.That(_recentProjects.Projects.Count(), Is.EqualTo(1));
        _dateTimeProvider.Verify(x => x.GetNowDateTime(), Times.Exactly(2));
        _configStorage.Verify(x => x.Save(new RecentProjectPoco { Id = 0, ProjectFullPath = "full path", LastAccess = new DateTime(2000, 1, 2) }));
    }

    [Test]
    public void OpenProject_MaxProjectsCount()
    {
        MakeRecentProjects();

        for (int i = 0; i < RecentProjects.MaxRecentProjectsCount; i++)
        {
            _project = new Mock<IProject>();
            _project.SetupGet(x => x.Root).Returns(new DirectoryProjectItem($"full path {i}", null));
            _dateTimeProvider.Setup(x => x.GetNowDateTime()).Returns(new DateTime(2000, 1, 1 + i));
            RaiseProjectOpened();
        }

        Assert.That(_recentProjects.Projects.Count(), Is.EqualTo(RecentProjects.MaxRecentProjectsCount));
        for (int i = 0; i < RecentProjects.MaxRecentProjectsCount; i++)
        {
            _configStorage.Verify(x => x.Save(new RecentProjectPoco { Id = i, ProjectFullPath = $"full path {i}", LastAccess = new DateTime(2000, 1, 1 + i) }), Times.Once());
        }
    }

    [Test]
    public void OpenProject_WithMaxRecentProjectsCount()
    {
        MakeRecentProjects();

        for (int i = 0; i < RecentProjects.MaxRecentProjectsCount; i++)
        {
            _project = new Mock<IProject>();
            _project.SetupGet(x => x.Root).Returns(new DirectoryProjectItem($"full path {i}", null));
            _dateTimeProvider.Setup(x => x.GetNowDateTime()).Returns(new DateTime(2000, 1, 1 + i));
            RaiseProjectOpened();
        }

        _project = new Mock<IProject>();
        _project.SetupGet(x => x.Root).Returns(new DirectoryProjectItem($"full path 6", null));
        _dateTimeProvider.Setup(x => x.GetNowDateTime()).Returns(new DateTime(2000, 1, 10));
        RaiseProjectOpened();

        Assert.That(_recentProjects.Projects.Count(), Is.EqualTo(RecentProjects.MaxRecentProjectsCount));
        // rewrite the oldest project
        _configStorage.Verify(x => x.Save(new RecentProjectPoco { Id = 0, ProjectFullPath = "full path 6", LastAccess = new DateTime(2000, 1, 10) }));
    }

    [Test]
    public void OpenProject_RaisePropertyChanged()
    {
        MakeRecentProjects();
        var projectsRaised = false;
        _recentProjects.PropertyChanged += (s, e) => projectsRaised = e.PropertyName == "Projects";

        RaiseProjectOpened();

        Assert.IsTrue(projectsRaised);
    }

    private void RaiseProjectOpened()
    {
        _projectLoader.Raise(x => x.ProjectOpened += null, new ProjectOpenedEventArgs(_project.Object));
    }

    private void MakeRecentProjects()
    {
        _recentProjects = new RecentProjects(_projectLoader.Object, _configStorage.Object, _dateTimeProvider.Object);
    }
}
