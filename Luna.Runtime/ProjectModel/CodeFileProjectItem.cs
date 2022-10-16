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

    internal void RaiseUpdateCodeModel(CodeModel oldCodeModel, CodeModelScopeIdentificatorsDifferent diff)
    {
        CodeModelUpdated?.Invoke(this, new(oldCodeModel, diff));
    }
}

public class CodeModelUpdatedEventArgs : EventArgs
{
    public CodeModel OldCodeModel { get; }
    public CodeModelScopeIdentificatorsDifferent Different { get; }

    public CodeModelUpdatedEventArgs(CodeModel oldCodeModel, CodeModelScopeIdentificatorsDifferent different)
    {
        OldCodeModel = oldCodeModel;
        Different = different;
    }
}
