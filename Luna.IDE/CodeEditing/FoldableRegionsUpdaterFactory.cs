using CodeHighlighter.Ancillary;
using CodeHighlighter.Core;
using Luna.Infrastructure;

namespace Luna.IDE.CodeEditing;

public interface IFoldableRegionsUpdaterFactory
{
    IFoldableRegionsUpdater Make(ILineFolds folds, ITokens tokens);
}

public class FoldableRegionsUpdaterFactory : IFoldableRegionsUpdaterFactory
{
    private readonly ILineFoldsItemsUpdater _lineFoldsItemsUpdater;
    private readonly IFoldableRegions _foldableRegions;
    private readonly ITimerManager _timerManager;

    public FoldableRegionsUpdaterFactory(
        ILineFoldsItemsUpdater lineFoldsItemsUpdater,
        IFoldableRegions foldableRegions,
        ITimerManager timerManager)
    {
        _lineFoldsItemsUpdater = lineFoldsItemsUpdater;
        _foldableRegions = foldableRegions;
        _timerManager = timerManager;
    }

    public IFoldableRegionsUpdater Make(ILineFolds folds, ITokens tokens)
    {
        return new FoldableRegionsUpdater(
            _lineFoldsItemsUpdater,
            _foldableRegions,
            _timerManager,
            folds,
            tokens);
    }
}
