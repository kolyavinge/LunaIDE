using System.Collections.Generic;

namespace Luna.ProjectModel;

public interface ICodeModelUpdater
{
    void SetCodeFiles(IEnumerable<CodeFileProjectItem> projectItems);

    void UpdateRequest();

    void UpdateNow();
}

public delegate void CodeModelUpdatedCallback(CodeModelUpdatedEventArgs e);
