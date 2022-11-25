using System.Threading.Tasks;
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
    private readonly ILineNumberProcessor _lineNumberProcessor;
    private int _oldTextLinesCount;
    private int _newTextLinesCount;
    private bool _inProgress;

    public IDiffCodeTextBox DiffCodeTextBox { get; }

    public LineNumberPanelModel OldLineNumberPanel { get; }

    public LineNumberPanelModel NewLineNumberPanel { get; }

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
        ILineNumberProcessor lineNumberProcessor)
    {
        _textDiffEngine = textDiffEngine;
        _textDiffCodeProviderFactory = textDiffCodeProviderFactory;
        _linesDecorationProcessor = linesDecorationProcessor;
        _lineNumberProcessor = lineNumberProcessor;
        DiffCodeTextBox = diffCodeTextBox;
        OldLineNumberPanel = new LineNumberPanelModel();
        NewLineNumberPanel = new LineNumberPanelModel();
    }

    public async Task MakeDiff(string fileExtension, string? oldFileText, string newFileText)
    {
        InProgress = true;
        var diffResult = await _textDiffEngine.GetSingleTextResultAsync(oldFileText ?? "", newFileText);
        var codeProvider = _textDiffCodeProviderFactory.Make(fileExtension, oldFileText ?? "", newFileText);
        InitDiffCodeTextBox(codeProvider, diffResult.VisualizerResult, oldFileText != null);
        InitNumberPanels(diffResult);
        InProgress = false;
    }

    public async Task MakeDiff(string? oldFileText, TextFileProjectItem newFile)
    {
        InProgress = true;
        var diffResult = await _textDiffEngine.GetSingleTextResultAsync(oldFileText ?? "", newFile.GetText());
        var codeProvider = _textDiffCodeProviderFactory.Make(oldFileText ?? "", newFile);
        InitDiffCodeTextBox(codeProvider, diffResult.VisualizerResult, oldFileText != null);
        InitNumberPanels(diffResult);
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
        _lineNumberProcessor.SetLineGaps(diffResult.VisualizerResult.LinesDiff, OldLineNumberPanel.Gaps, NewLineNumberPanel.Gaps);
    }
}
