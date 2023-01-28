using System.Threading.Tasks;
using CodeHighlighter;
using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Model;
using DiffTool.Visualization;
using Luna.IDE.Common;
using Luna.ProjectModel;
using Luna.Utils;

namespace Luna.IDE.TextDiff;

public class SingleTextDiff : NotificationObject, ISingleTextDiff
{
    private readonly ITextDiffEngine _textDiffEngine;
    private readonly ITextDiffCodeProviderFactory _textDiffCodeProviderFactory;
    private readonly ILinesDecorationProcessor _linesDecorationProcessor;
    private readonly ISingleTextDiffGapProcessor _gapProcessor;
    private int _oldTextLinesCount;
    private int _newTextLinesCount;
    private bool _inProgress;

    public IDiffCodeTextBox DiffCodeTextBox { get; }

    public ILineNumberPanelModel OldLineNumberPanel { get; }

    public ILineNumberPanelModel NewLineNumberPanel { get; }

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

    public bool InProgress
    {
        get => _inProgress;
        set { _inProgress = value; RaisePropertyChanged(() => InProgress); }
    }

    public SingleTextDiff(
        ITextDiffEngine textDiffEngine,
        ITextDiffCodeProviderFactory textDiffCodeProviderFactory,
        IDiffCodeTextBox diffCodeTextBox,
        ILinesDecorationProcessor linesDecorationProcessor,
        ISingleTextDiffGapProcessor gapProcessor)
    {
        _textDiffEngine = textDiffEngine;
        _textDiffCodeProviderFactory = textDiffCodeProviderFactory;
        _linesDecorationProcessor = linesDecorationProcessor;
        _gapProcessor = gapProcessor;
        DiffCodeTextBox = diffCodeTextBox;
        OldLineNumberPanel = LineNumberPanelModelFactory.MakeModel();
        NewLineNumberPanel = LineNumberPanelModelFactory.MakeModel();
    }

    public async Task MakeDiff(TextDiffResult diffResult, string fileExtension, string? oldFileText, string newFileText)
    {
        InProgress = true;
        var singleDiffResult = await _textDiffEngine.GetSingleTextResultAsync(diffResult);
        var codeProvider = _textDiffCodeProviderFactory.Make(fileExtension, oldFileText ?? "", newFileText);
        InitDiffCodeTextBox(codeProvider, singleDiffResult.VisualizerResult, oldFileText != null);
        InitNumberPanels(singleDiffResult);
        InProgress = false;
    }

    public async Task MakeDiff(TextDiffResult diffResult, string? oldFileText, TextFileProjectItem newFile)
    {
        InProgress = true;
        var singleDiffResult = await _textDiffEngine.GetSingleTextResultAsync(diffResult);
        var codeProvider = _textDiffCodeProviderFactory.Make(oldFileText ?? "", newFile);
        InitDiffCodeTextBox(codeProvider, singleDiffResult.VisualizerResult, oldFileText != null);
        InitNumberPanels(singleDiffResult);
        InProgress = false;
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
