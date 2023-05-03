using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Model;
using DiffTool.Visualization;
using Luna.IDE.Common;
using Luna.ProjectModel;
using Luna.Utils;

namespace Luna.IDE.TextDiff;

internal class SingleTextDiff : NotificationObject, ISingleTextDiff
{
    private readonly ITextDiffEngine _textDiffEngine;
    private readonly ITextDiffCodeProviderFactory _textDiffCodeProviderFactory;
    private readonly ILinesDecorationProcessor _linesDecorationProcessor;
    private readonly ISingleTextDiffGapProcessor _gapProcessor;
    private int _oldTextLinesCount;
    private int _newTextLinesCount;

    public IDiffCodeTextBox DiffCodeTextBox { get; }

    public ILineNumberPanel OldLineNumberPanel { get; }

    public ILineNumberPanel NewLineNumberPanel { get; }

    public int OldTextLinesCount
    {
        get => _oldTextLinesCount;
        private set { _oldTextLinesCount = value; RaisePropertyChanged(() => OldTextLinesCount!); }
    }

    public int NewTextLinesCount
    {
        get => _newTextLinesCount;
        private set { _newTextLinesCount = value; RaisePropertyChanged(() => NewTextLinesCount!); }
    }

    public SingleTextDiff(
        ITextDiffEngine textDiffEngine,
        ITextDiffCodeProviderFactory textDiffCodeProviderFactory,
        IDiffCodeTextBox diffCodeTextBox,
        ILineNumberPanel oldLineNumberPanel,
        ILineNumberPanel newLineNumberPanel,
        ILinesDecorationProcessor linesDecorationProcessor,
        ISingleTextDiffGapProcessor gapProcessor)
    {
        _textDiffEngine = textDiffEngine;
        _textDiffCodeProviderFactory = textDiffCodeProviderFactory;
        _linesDecorationProcessor = linesDecorationProcessor;
        _gapProcessor = gapProcessor;
        DiffCodeTextBox = diffCodeTextBox;
        OldLineNumberPanel = oldLineNumberPanel;
        NewLineNumberPanel = newLineNumberPanel;
    }

    public void MakeDiff(TextDiffResult diffResult, string fileExtension, string? oldFileText, string newFileText)
    {
        var singleDiffResult = _textDiffEngine.GetSingleTextResult(diffResult);
        var codeProvider = _textDiffCodeProviderFactory.Make(fileExtension, oldFileText ?? "", newFileText);
        InitDiffCodeTextBox(codeProvider, singleDiffResult.VisualizerResult, oldFileText != null);
        InitNumberPanels(singleDiffResult);
    }

    public void MakeDiff(TextDiffResult diffResult, string? oldFileText, TextFileProjectItem newFile)
    {
        var singleDiffResult = _textDiffEngine.GetSingleTextResult(diffResult);
        var codeProvider = _textDiffCodeProviderFactory.Make(oldFileText ?? "", newFile);
        InitDiffCodeTextBox(codeProvider, singleDiffResult.VisualizerResult, oldFileText != null);
        InitNumberPanels(singleDiffResult);
    }

    private void InitDiffCodeTextBox(ICodeProvider codeProvider, SingleTextVisualizerResult diffResult, bool hasOldFileText)
    {
        DiffCodeTextBox.Init(codeProvider, diffResult.Text);
        var firstChangeLineIndex = diffResult.LinesDiff.FindIndex(x => x.DiffKind != DiffTool.Core.DiffKind.Same);
        DiffCodeTextBox.GotoLine(firstChangeLineIndex != -1 ? firstChangeLineIndex : 0);
        if (hasOldFileText)
        {
            _linesDecorationProcessor.SetLineColors(diffResult.LinesDiff, DiffCodeTextBox.LinesDecoration);
        }
    }

    private void InitNumberPanels(SingleTextDiffResult diffResult)
    {
        OldLineNumberPanel.Gaps.Clear();
        NewLineNumberPanel.Gaps.Clear();
        OldTextLinesCount = diffResult.OldTextLinesCount;
        NewTextLinesCount = diffResult.NewTextLinesCount;
        _gapProcessor.SetLineGaps(diffResult.VisualizerResult.LinesDiff, OldLineNumberPanel.Gaps, NewLineNumberPanel.Gaps);
    }
}
