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
        _attached.Add(projectItem, new(projectItem, projectItem.CodeModel, updatedCallback));
    }

    public void Detach(CodeFileProjectItem projectItem)
    {
        _attached.Remove(projectItem);
    }

    public void OnTimerTick(object sender, EventArgs e)
    {
        _codeModelBuilder.BuildFor(_projectItems);
        _attached.Each(x => x.Value.RaiseCallback());
        _attached.Each(x => x.Value.UpdateOldCodeModel());
    }
}

class AttachedItem
{
    public readonly CodeFileProjectItem ProjectItem;
    public CodeModel OldCodeModel;
    public readonly CodeModelUpdatedCallback Callback;

    public AttachedItem(CodeFileProjectItem projectItem, CodeModel oldCodeModel, CodeModelUpdatedCallback callback)
    {
        ProjectItem = projectItem;
        OldCodeModel = oldCodeModel;
        Callback = callback;
    }

    public void RaiseCallback()
    {
        Callback(new(OldCodeModel, ProjectItem.CodeModel));
    }

    public void UpdateOldCodeModel()
    {
        OldCodeModel = ProjectItem.CodeModel;
    }
}
