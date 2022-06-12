using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Luna.IDE.Commands;
using Luna.IDE.Infrastructure;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;
using Luna.Utils;

namespace Luna.IDE.ViewModel;

public class ProjectTreeViewModel : NotificationObject
{
    private readonly IProjectExplorer _projectExplorer;
    private IEnumerable<ProjectTreeItem> _projectTreeItems;

    [Inject]
    public IProjectTreeItemOpenCommand? ProjectTreeItemOpenCommand { get; set; }

    public IEnumerable<ProjectTreeItem> ProjectTreeItems
    {
        get => _projectTreeItems;
        set
        {
            _projectTreeItems = value;
            RaisePropertyChanged(() => ProjectTreeItems);
        }
    }

    public ProjectTreeViewModel(IProjectExplorer projectExplorer)
    {
        _projectExplorer = projectExplorer;
        _projectExplorer.ProjectOpened += OnProjectOpened;
        _projectTreeItems = Enumerable.Empty<ProjectTreeItem>();
    }

    private void OnProjectOpened(object? sender, EventArgs e)
    {
        UpdateProjectTreeItems();
    }

    private void OnTreeItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsExpanded")
        {
            UpdateProjectTreeItems();
        }
    }

    private void UpdateProjectTreeItems()
    {
        ProjectTreeItems.Each(x => x.PropertyChanged -= OnTreeItemPropertyChanged);
        ProjectTreeItems = MakeFlatList(_projectExplorer.ProjectTreeRoot!).ToList();
        ProjectTreeItems.Each(x => x.PropertyChanged += OnTreeItemPropertyChanged);
    }

    private List<ProjectTreeItem> MakeFlatList(ProjectTreeItem item)
    {
        var result = new List<ProjectTreeItem> { item };
        if (item.IsExpanded)
        {
            item.Children.Each(child => result.AddRange(MakeFlatList(child)));
        }

        return result;
    }
}
