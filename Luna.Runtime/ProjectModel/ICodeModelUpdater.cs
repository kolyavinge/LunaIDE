using System.Collections.Generic;

namespace Luna.ProjectModel;

public interface ICodeModelUpdater
{
    void SetCodeFiles(IEnumerable<CodeFileProjectItem> projectItems);

    void UpdateRequest();
}

public delegate void CodeModelUpdatedCallback(CodeModelUpdatedEventArgs e);
