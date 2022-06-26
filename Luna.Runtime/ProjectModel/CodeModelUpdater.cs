using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Infrastructure;
using Luna.Parsing;
using Luna.Utils;

namespace Luna.ProjectModel;

public class CodeModelUpdater : ICodeModelUpdater
{
    private readonly ICodeModelBuilder _codeModelBuilder;
    private readonly Dictionary<CodeFileProjectItem, AttachedItem> _attached = new();
    private List<CodeFileProjectItem> _projectItems = new();

    public CodeModelUpdater(ITimerManager timerManager) : this(timerManager, new CodeModelBuilder()) { }

    internal CodeModelUpdater(ITimerManager timerManager, ICodeModelBuilder codeModelBuilder)
    {
        _codeModelBuilder = codeModelBuilder;
        timerManager.CreateNew(TimeSpan.FromSeconds(2), OnTimerTick);
    }

    public void SetCodeFiles(IEnumerable<CodeFileProjectItem> projectItems)
    {
        _projectItems = projectItems.ToList();
        _attached.Clear();
    }

    public void Attach(CodeFileProjectItem projectItem, CodeModelUpdatedCallback updatedCallback)
    {
        var scope = new CodeModelScope();
        _attached.Add(projectItem, new(projectItem, scope.GetScopeIdentificators(projectItem.CodeModel), updatedCallback));
    }

    public void Detach(CodeFileProjectItem projectItem)
    {
        _attached.Remove(projectItem);
    }

    public void OnTimerTick(object? sender, EventArgs e)
    {
        _codeModelBuilder.BuildFor(_projectItems);
        _attached.Each(x => x.Value.RaiseCallback());
    }
}

class AttachedItem
{
    private readonly CodeFileProjectItem _projectItem;
    private CodeModelScopeIdentificators _oldScopeIdentificators;
    private readonly CodeModelUpdatedCallback _callback;

    public AttachedItem(CodeFileProjectItem projectItem, CodeModelScopeIdentificators oldScopeIdentificators, CodeModelUpdatedCallback callback)
    {
        _projectItem = projectItem;
        _oldScopeIdentificators = oldScopeIdentificators;
        _callback = callback;
    }

    public void RaiseCallback()
    {
        var scope = new CodeModelScope();
        var newScopeIdentificators = scope.GetScopeIdentificators(_projectItem.CodeModel);
        var diff = IdentificatorsComparator.GetDifferent(_oldScopeIdentificators, newScopeIdentificators);
        _callback(new(_projectItem.CodeModel, diff));
        _oldScopeIdentificators = newScopeIdentificators;
    }
}
