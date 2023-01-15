using System.Collections.Generic;
using System.IO;
using System.Linq;
using Luna.IDE.Common;
using Luna.IDE.ProjectExploration;

namespace Luna.IDE.Configuration;

public interface IRecentProjects
{
    IEnumerable<RecentProject> Projects { get; }
    bool AnyProjects { get; }
}

public class RecentProject
{
    public string FullPath { get; set; }
    public string Name { get; set; }

    public RecentProject(string fullPath, string name)
    {
        FullPath = fullPath;
        Name = name;
    }
}

public class RecentProjects : NotificationObject, IRecentProjects
{
    public const int MaxRecentProjectsCount = 5;

    private readonly IConfigStorage _configStorage;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly List<RecentProjectPoco> _recentProjects;

    public RecentProjects(
        IProjectLoader projectLoader,
        IConfigStorage configStorage,
        IDateTimeProvider dateTimeProvider)
    {
        projectLoader.ProjectOpened += OnProjectOpened;
        _configStorage = configStorage;
        _dateTimeProvider = dateTimeProvider;
        _recentProjects = new List<RecentProjectPoco>();
        LoadRecentProjects();
    }

    public IEnumerable<RecentProject> Projects
    {
        get => _recentProjects.Select(x => new RecentProject(x.ProjectFullPath, Path.GetFileName(x.ProjectFullPath))).OrderBy(x => x.Name);
    }

    public bool AnyProjects
    {
        get => _recentProjects.Any();
    }

    private void OnProjectOpened(object? sender, ProjectOpenedEventArgs e)
    {
        var recentProject = _recentProjects.FirstOrDefault(x => x.ProjectFullPath == e.Project.Root.FullPath);
        if (recentProject != null)
        {
            recentProject.LastAccess = _dateTimeProvider.GetNowDateTime();
        }
        else
        {
            if (_recentProjects.Count < MaxRecentProjectsCount)
            {
                recentProject = new RecentProjectPoco();
                recentProject.Id = _recentProjects.Count;
                recentProject.ProjectFullPath = e.Project.Root.FullPath;
                recentProject.LastAccess = _dateTimeProvider.GetNowDateTime();
                _recentProjects.Add(recentProject);
            }
            else
            {
                recentProject = _recentProjects.OrderBy(x => x.LastAccess).First();
                recentProject.ProjectFullPath = e.Project.Root.FullPath;
                recentProject.LastAccess = _dateTimeProvider.GetNowDateTime();
            }
            RaisePropertyChanged(() => Projects);
        }
        _configStorage.Save(recentProject);
    }

    private void LoadRecentProjects()
    {
        for (int i = 0; i < MaxRecentProjectsCount; i++)
        {
            var rp = _configStorage.GetById<RecentProjectPoco>(i);
            if (rp != null) _recentProjects.Add(rp);
            else break;
        }
    }
}
