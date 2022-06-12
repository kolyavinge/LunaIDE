using System;
using System.Collections.Generic;

namespace Luna.ProjectModel;

public interface ICodeModelUpdater
{
    void SetCodeFiles(IEnumerable<CodeFileProjectItem> projectItems);

    void Attach(CodeFileProjectItem projectItem, CodeModelUpdatedCallback updatedCallback);

    void Detach(CodeFileProjectItem projectItem);
}

public class CodeModelUpdatedEventArgs : EventArgs
{
    public CodeModel OldModel { get; }

    public CodeModel NewModel { get; }

    internal CodeModelUpdatedEventArgs(CodeModel oldModel, CodeModel newModel)
    {
        OldModel = oldModel;
        NewModel = newModel;
    }
}

public delegate void CodeModelUpdatedCallback(CodeModelUpdatedEventArgs e);
