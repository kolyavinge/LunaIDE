using System;
using System.Collections.Generic;

namespace Luna.ProjectModel;

public interface ICodeModelUpdater
{
    void SetCodeFiles(IEnumerable<CodeFileProjectItem> projectItems);

    void Attach(CodeFileProjectItem projectItem, CodeModelUpdatedCallback updatedCallback);

    void Detach(CodeFileProjectItem projectItem);

    void UpdateRequest();
}

public class CodeModelUpdatedEventArgs : EventArgs
{
    public CodeModel NewModel { get; }
    public CodeModelScopeIdentificatorsDifferent Different { get; }

    internal CodeModelUpdatedEventArgs(CodeModel newModel, CodeModelScopeIdentificatorsDifferent different)
    {
        NewModel = newModel;
        Different = different;
    }
}

public delegate void CodeModelUpdatedCallback(CodeModelUpdatedEventArgs e);
