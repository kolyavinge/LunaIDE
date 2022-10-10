using System.Collections.Generic;
using System.Linq;

namespace Luna.IDE.Versioning;

public abstract class AbstractDirectory<TDirectory, TFile> where TDirectory : AbstractDirectory<TDirectory, TFile>
{
    private readonly List<TDirectory> _innerDirectories;
    private readonly List<TFile> _innerFiles;
    private readonly Func<string, TDirectory> _makeDirectoryFunc;
    private readonly Func<TFile, string> _relativePathFunc;

    public string Name { get; }

    public IReadOnlyCollection<TDirectory> InnerDirectories => _innerDirectories;

    public IReadOnlyCollection<TFile> InnerFiles => _innerFiles;

    public IReadOnlyCollection<TFile> AllFiles
    {
        get
        {
            var result = new List<TFile>(_innerFiles);
            foreach (var dir in _innerDirectories) result.AddRange(dir.AllFiles);
            return result;
        }
    }

    public event EventHandler? ChildrenRefreshed;

    protected AbstractDirectory(string name, Func<string, TDirectory> makeDirectoryFunc, Func<TFile, string> relativePathFunc)
    {
        Name = name;
        _makeDirectoryFunc = makeDirectoryFunc;
        _relativePathFunc = relativePathFunc;
        _innerDirectories = new List<TDirectory>();
        _innerFiles = new List<TFile>();
    }

    public virtual void AddFiles(IEnumerable<TFile> versionedFiles)
    {
        foreach (var versionedFile in versionedFiles)
        {
            var parent = this;
            var directories = _relativePathFunc(versionedFile).Split('\\').SkipLast(1).ToList();
            foreach (var directory in directories)
            {
                var child = parent.InnerDirectories.FirstOrDefault(x => x.Name == directory);
                if (child == null)
                {
                    child = _makeDirectoryFunc(directory);
                    parent._innerDirectories.Add(child);
                }
                parent = child;
            }
            parent._innerFiles.Add(versionedFile);
        }

        ChildrenRefreshed?.Invoke(this, EventArgs.Empty);
    }

    public virtual void DeleteFiles(IEnumerable<TFile> versionedFiles)
    {
        foreach (var versionedFile in versionedFiles)
        {
            var parent = this;
            var directories = _relativePathFunc(versionedFile).Split('\\').SkipLast(1).ToList();
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

    private bool DeleteEmptyDirectories(AbstractDirectory<TDirectory, TFile> parent)
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
