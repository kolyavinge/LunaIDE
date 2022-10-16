using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Luna.Infrastructure;

namespace Luna.ProjectModel;

public class CodeModelUpdater : ICodeModelUpdater
{
    private readonly ICodeModelBuilder _codeModelBuilder;
    private List<CodeFileProjectItem> _projectItems = new();
    private int _updateRequest;

    public CodeModelUpdater(ITimerManager timerManager) : this(timerManager, new CodeModelBuilder()) { }

    internal CodeModelUpdater(ITimerManager timerManager, ICodeModelBuilder codeModelBuilder)
    {
        _codeModelBuilder = codeModelBuilder;
        timerManager.CreateAndStart(TimeSpan.FromSeconds(2), OnTimerTick);
    }

    public void SetCodeFiles(IEnumerable<CodeFileProjectItem> projectItems)
    {
        _projectItems = projectItems.ToList();
        UpdateRequest();
    }

    public void UpdateRequest()
    {
        Interlocked.Exchange(ref _updateRequest, 1);
    }

    public void OnTimerTick(object? sender, EventArgs e)
    {
        if (_updateRequest == 0) return;
        Interlocked.Exchange(ref _updateRequest, 0);
        _codeModelBuilder.BuildFor(_projectItems);
    }
}
