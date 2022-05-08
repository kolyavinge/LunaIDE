using Luna.Infrastructure;

namespace Luna.ProjectModel
{
    public class CodeFileProjectItem : FileProjectItem
    {
        public CodeModel? CodeModel { get; internal set; }

        internal CodeFileProjectItem(string fullPath, DirectoryProjectItem? parent, IFileSystem fileSystem) : base(fullPath, parent, fileSystem)
        {
        }
    }
}
