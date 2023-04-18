using System.Linq;
using CodeHighlighter.Ancillary;
using CodeHighlighter.Core;
using Luna.Infrastructure;

namespace Luna.IDE.CodeEditing;

internal interface IFoldableRegionsUpdater
{
    void Request();
}

internal class FoldableRegionsUpdater : DelayedAction, IFoldableRegionsUpdater
{
    private readonly ILineFoldsItemsUpdater _lineFoldsItemsUpdater;
    private readonly IFoldableRegions _foldableRegions;
    private readonly ILineFolds _folds;
    private readonly ITokens _tokens;

    public FoldableRegionsUpdater(
        ILineFoldsItemsUpdater lineFoldsItemsUpdater,
        IFoldableRegions foldableRegions,
        ITimerManager timerManager,
        ILineFolds folds,
        ITokens tokens)
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
