using System.Collections.Generic;
using System.Linq;
using Luna.IDE.Mvvm;
using Luna.IDE.Utils;
using Luna.ProjectModel;

namespace Luna.IDE.Model
{
    public class ProjectTreeItem : NotificationObject
    {
        private bool _isExpanded;
        private bool _isSelected;

        public ProjectItem ProjecItem { get; }

        public string Name => ProjecItem.Name;

        public bool HasChildren => Children != null && Children.Any();

        public IReadOnlyCollection<ProjectTreeItem> Children { get; }

        public ProjectTreeItemKind Kind { get; }

        public int Depth { get; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                RaisePropertyChanged(() => IsExpanded);
                if (!_isExpanded && GetAllChildren().Any(x => x.IsSelected))
                {
                    GetAllChildren().Each(c => c.IsSelected = false);
                    IsSelected = true;
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }

        public ProjectTreeItem(ProjectItem projecItem)
        {
            ProjecItem = projecItem;
            Children = ProjecItem is DirectoryProjectItem
                ? ((DirectoryProjectItem)ProjecItem).Children.Select(i => new ProjectTreeItem(i)).ToList()
                : new List<ProjectTreeItem>();
            Kind = GetKind();
            Depth = GetDepth();
            IsExpanded = projecItem.Parent == null;
            IsSelected = false;
        }

        public List<ProjectTreeItem> GetAllChildren()
        {
            var result = new List<ProjectTreeItem>();
            result.Add(this);
            Children.Each(child => result.AddRange(child.GetAllChildren()));

            return result;
        }

        private int GetDepth()
        {
            var parent = ProjecItem.Parent;
            if (parent == null) return 0;
            int depth = -1;
            while (parent != null) { parent = parent.Parent; depth++; }

            return depth;
        }

        private ProjectTreeItemKind GetKind()
        {
            if (ProjecItem is DirectoryProjectItem && ProjecItem.Parent == null) return ProjectTreeItemKind.Project;
            if (ProjecItem is DirectoryProjectItem) return ProjectTreeItemKind.Directory;
            if (ProjecItem is CodeFileProjectItem) return ProjectTreeItemKind.CodeFile;

            return ProjectTreeItemKind.Unknown;
        }
    }

    public enum ProjectTreeItemKind : int
    {
        Unknown = 0,
        Project = 1,
        Directory = 2,
        CodeFile = 3
    }
}
