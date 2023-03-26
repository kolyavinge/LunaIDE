using System.Linq;
using CodeHighlighter.Model;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

public interface IFoldableRegionsUpdater
{
    void Update(ILineFolds folds, CodeModel codeModel);
}

public class FoldableRegionsUpdater : IFoldableRegionsUpdater
{
    private readonly ILineFoldsItemsUpdater _lineFoldsItemsUpdater;
    private readonly IFoldableRegions _foldableRegions;

    public FoldableRegionsUpdater(
        ILineFoldsItemsUpdater lineFoldsItemsUpdater,
        IFoldableRegions foldableRegions)
    {
        _lineFoldsItemsUpdater = lineFoldsItemsUpdater;
        _foldableRegions = foldableRegions;
    }

    public void Update(ILineFolds folds, CodeModel codeModel)
    {
        var items = _foldableRegions.GetRegions(codeModel).Select(x => new LineFold(x.LineIndex, x.LinesCount)).ToList();
        _lineFoldsItemsUpdater.Update(folds, items);
    }
}
