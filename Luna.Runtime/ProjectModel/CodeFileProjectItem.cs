using Luna.Infrastructure;

namespace Luna.ProjectModel;

public class CodeFileProjectItem : TextFileProjectItem
{
    public CodeModel CodeModel { get; internal set; }

    public event EventHandler<CodeModelUpdatedEventArgs>? CodeModelUpdated;

    internal CodeFileProjectItem(string fullPath, DirectoryProjectItem? parent, IFileSystem fileSystem)
        : base(fullPath, parent, fileSystem)
    {
        CodeModel = new CodeModel();
    }

    internal void RaiseUpdateCodeModel(CodeModelScopeIdentificatorsDifferent diff)
    {
        CodeModelUpdated?.Invoke(this, new(diff));
    }
}

public class CodeModelUpdatedEventArgs : EventArgs
{
    public CodeModelScopeIdentificatorsDifferent Different { get; }

    public CodeModelUpdatedEventArgs(CodeModelScopeIdentificatorsDifferent different)
    {
        Different = different;
    }
}
