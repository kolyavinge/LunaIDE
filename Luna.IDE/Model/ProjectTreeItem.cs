using System;
using System.Collections.Generic;
using System.Linq;
using Luna.IDE.Controls.Tree;
using Luna.ProjectModel;

namespace Luna.IDE.Model;

public class ProjectTreeItem : TreeItem
{
    public ProjectItem ProjecItem { get; }

    public ProjectTreeItem(ProjectTreeItem? parent, ProjectItem projecItem) :
        base(parent, projecItem.Name, GetImageFileName(projecItem))
    {
        ProjecItem = projecItem;
    }

    protected override IEnumerable<TreeItem> GetChildren()
    {
        return ProjecItem is DirectoryProjectItem dir
            ? dir.Children.Select(i => new ProjectTreeItem(this, i)).ToArray()
            : new ProjectTreeItem[0];
    }

    private static string GetImageFileName(ProjectItem projecItem)
    {
        if (projecItem is DirectoryProjectItem && projecItem.Parent == null) return "project.png";
        if (projecItem is DirectoryProjectItem) return "directory.png";
        if (projecItem is CodeFileProjectItem) return "codefile.png";

        throw new ArgumentException();
    }
}
