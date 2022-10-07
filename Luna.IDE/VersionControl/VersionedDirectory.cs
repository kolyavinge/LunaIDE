using System.Collections.Generic;
using System.Linq;
using VersionControl.Core;

namespace Luna.IDE.VersionControl;

public class VersionedDirectory
{
    private readonly List<VersionedDirectory> _innerDirectories;
    private readonly List<VersionedFile> _innerFiles;

    public string Name { get; }

    public IReadOnlyCollection<VersionedDirectory> InnerDirectories => _innerDirectories;

    public IReadOnlyCollection<VersionedFile> InnerFiles => _innerFiles;

    public IReadOnlyCollection<VersionedFile> AllFiles
    {
        get
        {
            var result = new List<VersionedFile>(_innerFiles);
            foreach (var dir in _innerDirectories) result.AddRange(dir.AllFiles);
            return result;
        }
    }

    public event EventHandler? ChildrenRefreshed;

    public VersionedDirectory(string name)
    {
        _innerDirectories = new List<VersionedDirectory>();
        _innerFiles = new List<VersionedFile>();
        Name = name;
    }

    public void AddFiles(IEnumerable<VersionedFile> versionedFiles)
    {
        foreach (var versionedFile in versionedFiles)
        {
            var parent = this;
            var directories = versionedFile.RelativePath.Split('\\').SkipLast(1).ToList();
            foreach (var directiory in directories)
            {
                var child = parent.InnerDirectories.FirstOrDefault(x => x.Name == directiory);
                if (child == null)
                {
                    child = new VersionedDirectory(directiory);
                    parent._innerDirectories.Add(child);
                }
                parent = child;
            }
            parent._innerFiles.Add(versionedFile);
        }

        ChildrenRefreshed?.Invoke(this, EventArgs.Empty);
    }

    public void DeleteFiles(IEnumerable<VersionedFile> versionedFiles)
    {
        foreach (var versionedFile in versionedFiles)
        {
            var parent = this;
            var directories = versionedFile.RelativePath.Split('\\').SkipLast(1).ToList();
            var fileFinded = true;
            foreach (var directiory in directories)
            {
                var child = parent.InnerDirectories.FirstOrDefault(x => x.Name == directiory);
                if (child == null)
                {
                    fileFinded = false;
                    break;
                }
                parent = child;
            }
            if (fileFinded)
            {
                parent._innerFiles.Remove(versionedFile);
            }
        }

        DeleteEmptyDirectories(this);

        ChildrenRefreshed?.Invoke(this, EventArgs.Empty);
    }

    private bool DeleteEmptyDirectories(VersionedDirectory parent)
    {
        for (int index = 0; index < parent._innerDirectories.Count;)
        {
            var directory = parent._innerDirectories[index];
            if (DeleteEmptyDirectories(directory))
            {
                parent._innerDirectories.RemoveAt(index);
            }
            else
            {
                index++;
            }
        }

        return !parent.InnerDirectories.Any() && !parent.InnerFiles.Any();
    }
}
