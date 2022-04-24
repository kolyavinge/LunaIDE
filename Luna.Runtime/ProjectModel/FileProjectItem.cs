using System.IO;
using Luna.Infrastructure;

namespace Luna.ProjectModel
{
    public abstract class FileProjectItem : ProjectItem
    {
        protected readonly IFileSystem _fileSystem;

        internal FileProjectItem(string fullPath, ProjectItem? parent, IFileSystem fileSystem) : base(Path.GetFileName(fullPath), fullPath, parent)
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
}
