using System.Collections.Generic;
using System.Linq;
using Luna.Infrastructure;
using Luna.Output;

namespace Luna.ProjectModel;

public class CodeModelUpdater : DelayedAction, ICodeModelUpdater
{
    private readonly ICodeModelBuilder _codeModelBuilder;
    private List<CodeFileProjectItem> _projectItems = new();

    public CodeModelUpdater(ITimerManager timerManager) : this(timerManager, new CodeModelBuilder(new EmptyOutputWriter())) { }

    internal CodeModelUpdater(ITimerManager timerManager, ICodeModelBuilder codeModelBuilder)
        : base(timerManager)
    {
        _codeModelBuilder = codeModelBuilder;
    }

    public void SetCodeFiles(IEnumerable<CodeFileProjectItem> projectItems)
    {
        _projectItems = projectItems.ToList();
        Request();
    }

    public void UpdateNow() => InnerDo();

    protected override void InnerDo() => _codeModelBuilder.BuildFor(_projectItems);
}
