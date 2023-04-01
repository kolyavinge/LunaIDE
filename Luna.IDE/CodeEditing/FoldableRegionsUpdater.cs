using System.Linq;
using CodeHighlighter.Model;
using Luna.Infrastructure;

namespace Luna.IDE.CodeEditing;

public interface IFoldableRegionsUpdater
{
    void Request();
}

public class FoldableRegionsUpdater : DelayedAction, IFoldableRegionsUpdater
{
    private readonly ILineFoldsItemsUpdater _lineFoldsItemsUpdater;
    private readonly IFoldableRegions _foldableRegions;
    private readonly ILineFolds _folds;
    private readonly ITokenCollection _tokens;

    public FoldableRegionsUpdater(
        ILineFoldsItemsUpdater lineFoldsItemsUpdater,
        IFoldableRegions foldableRegions,
        ITimerManager timerManager,
        ILineFolds folds,
        ITokenCollection tokens)
        : base(timerManager)
    {
        _lineFoldsItemsUpdater = lineFoldsItemsUpdater;
        _foldableRegions = foldableRegions;
        _folds = folds;
        _tokens = tokens;
    }

    protected override void InnerDo()
    {
        var items = _foldableRegions.GetRegions(_tokens).Select(x => new LineFold(x.LineIndex, x.LinesCount)).ToList();
        _lineFoldsItemsUpdater.Update(_folds, items);
    }
}
