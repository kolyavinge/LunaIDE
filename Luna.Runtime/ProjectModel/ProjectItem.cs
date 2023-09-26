using System.Collections.Generic;

namespace Luna.ProjectModel;

public abstract class ProjectItem
{
    public string Name { get; }

    public string FullPath { get; }

    public ProjectItem? Parent { get; }

    protected internal ProjectItem(string name, string fullPath, ProjectItem? parent)
    {
        Name = name;
        FullPath = fullPath;
        Parent = parent;
    }

    public string PathFromRoot
    {
        get
        {
            ProjectItem? item = this;
            var path = new Stack<string>();
            while (item is not null)
            {
                path.Push(item.Name);
                item = item.Parent;
            }
            path.Pop();

            return String.Join("\\", path);
        }
    }

    public IReadOnlyCollection<ProjectItem> AllParents
    {
        get
        {
            var parents = new Stack<ProjectItem>();
            var parent = Parent;
            while (parent is not null)
            {
                parents.Push(parent);
                parent = parent.Parent;
            }

            return parents;
        }
    }
}
