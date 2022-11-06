using System.IO;
using System.Linq;
using Luna.Infrastructure;

namespace Luna.ProjectModel;

public interface IProject
{
    DirectoryProjectItem Root { get; }
    void AddItem(DirectoryProjectItem parent, ProjectItem item);
    ProjectItem? FindItemByPath(string fullOrRelativePath);
}

public class Project : IProject
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

    public ProjectItem? FindItemByPath(string fullOrRelativePath)
    {
        if (!fullOrRelativePath.StartsWith(Root.FullPath))
        {
            fullOrRelativePath = Path.Combine(Root.FullPath, fullOrRelativePath);
        }
        var pathFromRoot = fullOrRelativePath.Substring(Root.FullPath.Length + 1);
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
