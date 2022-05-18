using System;
using System.Collections.Generic;
using System.IO;
using Luna.Infrastructure;

namespace Luna.ProjectModel;

public abstract class FileProjectItem : ProjectItem
{
    protected readonly IFileSystem _fileSystem;

    public string PathFromRoot
    {
        get
        {
            ProjectItem? item = this;
            var path = new Stack<string>();
            while (item != null)
            {
                path.Push(item.Name);
                item = item.Parent;
            }
            path.Pop();

            return String.Join("\\", path);
        }
    }

    internal FileProjectItem(string fullPath, DirectoryProjectItem? parent, IFileSystem fileSystem) : base(Path.GetFileName(fullPath), fullPath, parent)
    {
        _fileSystem = fileSystem;
    }

    public string GetText()
    {
        return _fileSystem.ReadFileText(FullPath);
    }

    public void SaveText(string text)
    {
        _fileSystem.SaveFileText(FullPath, text);
    }
}
