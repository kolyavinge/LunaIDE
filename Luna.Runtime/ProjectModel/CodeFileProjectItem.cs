using Luna.Infrastructure;

namespace Luna.ProjectModel;

public class CodeFileProjectItem : TextFileProjectItem
{
    public CodeModel CodeModel { get; internal set; }

    internal CodeFileProjectItem(string fullPath, DirectoryProjectItem? parent, IFileSystem fileSystem) : base(fullPath, parent, fileSystem)
    {
        CodeModel = new CodeModel();
    }
}
