using System.IO;
using Luna.Infrastructure;

namespace Luna.ProjectModel;

public abstract class TextFileProjectItem : ProjectItem
{
    protected readonly IFileSystem _fileSystem;
    private ITextGettingStrategy _textGettingStrategy;

    internal TextFileProjectItem(string fullPath, DirectoryProjectItem? parent, IFileSystem fileSystem) : base(Path.GetFileName(fullPath), fullPath, parent)
    {
        _fileSystem = fileSystem;
        _textGettingStrategy = new FileTextGettingStrategy(FullPath, _fileSystem);
    }

    public string GetText()
    {
        return _textGettingStrategy.GetText();
    }

    public void SaveText(string text)
    {
        _fileSystem.SaveFileText(FullPath, text);
    }

    public void ResetTextGettingStrategy()
    {
        _textGettingStrategy = new FileTextGettingStrategy(FullPath, _fileSystem);
    }

    public void SetTextGettingStrategy(ITextGettingStrategy textGettingStrategy)
    {
        _textGettingStrategy = textGettingStrategy;
    }

    public interface ITextGettingStrategy
    {
        string GetText();
    }

    class FileTextGettingStrategy : ITextGettingStrategy
    {
        private readonly string _fullPath;
        private readonly IFileSystem _fileSystem;

        public FileTextGettingStrategy(string fullPath, IFileSystem fileSystem)
        {
            _fullPath = fullPath;
            _fileSystem = fileSystem;
        }

        public string GetText()
        {
            return _fileSystem.ReadFileText(_fullPath);
        }
    }
}
