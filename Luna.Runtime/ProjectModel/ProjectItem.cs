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
}
