using System.Linq;
using Luna.Infrastructure;

namespace Luna.ProjectModel;

public class Project
{
    public static Project Open(string fullPath, IFileSystem fileSystem) => new(fullPath, fileSystem);

    private readonly string _fullPath;
    private readonly IFileSystem _fileSystem;

    public DirectoryProjectItem Root { get; }

    internal Project(string fullPath, IFileSystem fileSystem)
    {
        _fullPath = fullPath;
        _fileSystem = fileSystem;
        Root = new DirectoryProjectItem(_fullPath, null);
        ReadDirectory(_fullPath, Root);
    }

    private void ReadDirectory(string parentPath, DirectoryProjectItem parent)
    {
        foreach (var directoryPath in _fileSystem.GetDirectories(parentPath))
        {
            var directory = new DirectoryProjectItem(directoryPath, parent);
            parent.AddChild(directory);
            ReadDirectory(directoryPath, directory);
        }
        foreach (var directoryPath in _fileSystem.GetFiles(parentPath, "*.luna"))
        {
            var codeFile = new CodeFileProjectItem(directoryPath, parent, _fileSystem);
            parent.AddChild(codeFile);
        }
    }

    public void AddItem(DirectoryProjectItem parent, ProjectItem item)
    {
        parent.AddChild(item);
    }

    public ProjectItem? FindItemByPath(string fullPath)
    {
        if (!fullPath.StartsWith(Root.FullPath)) return null;
        var pathFromRoot = fullPath.Substring(Root.FullPath.Length + 1);
        var splittedPath = pathFromRoot.Split('\\');
        ProjectItem? result = Root;
        foreach (var itemName in splittedPath)
        {
            if (result == null) continue;
            if (result.Name == itemName) break;
            if (result is DirectoryProjectItem directory)
            {
                result = directory.Children.FirstOrDefault(x => x.Name == itemName);
            }
        }

        return result;
    }
}
