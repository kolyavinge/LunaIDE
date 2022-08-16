using Luna.Infrastructure;

namespace Luna.ProjectModel;

public class CodeFileProjectItem : TextFileProjectItem
{
    public CodeModel CodeModel { get; internal set; }

    public event EventHandler? CodeModelUpdated;

    internal CodeFileProjectItem(string fullPath, DirectoryProjectItem? parent, IFileSystem fileSystem) : base(fullPath, parent, fileSystem)
    {
        CodeModel = new CodeModel();
    }

    internal void RaiseUpdateCodeModel()
    {
        CodeModelUpdated?.Invoke(this, EventArgs.Empty);
    }
}
