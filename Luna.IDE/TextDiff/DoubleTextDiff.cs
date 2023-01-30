using CodeHighlighter;
using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Model;
using Luna.IDE.Common;
using Luna.ProjectModel;
using Luna.Utils;

namespace Luna.IDE.TextDiff;

public class DoubleTextDiff : NotificationObject, IDoubleTextDiff
{
    private readonly ITextDiffCodeProviderFactory _textDiffCodeProviderFactory;
    private readonly ILinesDecorationProcessor _linesDecorationProcessor;
    private readonly IDoubleTextDiffGapProcessor _gapProcessor;
    private int _oldTextLinesCount;
    private int _newTextLinesCount;
    private bool _inProgress;

    public IDiffCodeTextBox OldDiffCodeTextBox { get; }

    public IDiffCodeTextBox NewDiffCodeTextBox { get; }

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

    public DoubleTextDiff(
        ITextDiffCodeProviderFactory textDiffCodeProviderFactory,
        IDiffCodeTextBox oldDiffCodeTextBox,
        IDiffCodeTextBox newDiffCodeTextBox,
        ILinesDecorationProcessor linesDecorationProcessor,
        IDoubleTextDiffGapProcessor gapProcessor)
    {
        _textDiffCodeProviderFactory = textDiffCodeProviderFactory;
        OldDiffCodeTextBox = oldDiffCodeTextBox;
        NewDiffCodeTextBox = newDiffCodeTextBox;
        _linesDecorationProcessor = linesDecorationProcessor;
        _gapProcessor = gapProcessor;
        OldLineNumberPanel = LineNumberPanelModelFactory.MakeModel();
        NewLineNumberPanel = LineNumberPanelModelFactory.MakeModel();
    }

    public void MakeDiff(TextDiffResult diffResult, string? oldFileText, TextFileProjectItem newFile)
    {
        InProgress = true;
        var codeProvider = _textDiffCodeProviderFactory.Make(oldFileText ?? "", newFile);
        InitDiffCodeTextBox(codeProvider, diffResult, oldFileText ?? "", newFile.GetText(), oldFileText != null);
        InitGaps(diffResult);
        SynchronizeHorizontalScrollbars();
        InProgress = false;
    }

    private void InitDiffCodeTextBox(ICodeProvider codeProvider, TextDiffResult diffResult, string oldFileText, string newFileText, bool hasOldFileText)
    {
        OldDiffCodeTextBox.Init(codeProvider, oldFileText);
        NewDiffCodeTextBox.Init(codeProvider, newFileText);
        var firstChangeLineIndex = diffResult.LinesDiff.FindIndex(x => x.Kind != DiffTool.Core.DiffKind.Same);
        OldDiffCodeTextBox.GotoLine(firstChangeLineIndex != -1 ? firstChangeLineIndex : 0);
        NewDiffCodeTextBox.GotoLine(firstChangeLineIndex != -1 ? firstChangeLineIndex : 0);
        if (hasOldFileText)
        {
            _linesDecorationProcessor.SetLineColors(diffResult.LinesDiff, OldDiffCodeTextBox.LinesDecoration, NewDiffCodeTextBox.LinesDecoration);
        }
    }

    private void InitGaps(TextDiffResult diffResult)
    {
        OldLineNumberPanel.Gaps.Clear();
        NewLineNumberPanel.Gaps.Clear();
        OldDiffCodeTextBox.Gaps.Clear();
        NewDiffCodeTextBox.Gaps.Clear();
        OldTextLinesCount = diffResult.OldText.Lines.Count;
        NewTextLinesCount = diffResult.NewText.Lines.Count;
        _gapProcessor.SetLineGaps(diffResult.LinesDiff, OldLineNumberPanel.Gaps, NewLineNumberPanel.Gaps, OldDiffCodeTextBox.Gaps, NewDiffCodeTextBox.Gaps);
    }

    private void SynchronizeHorizontalScrollbars()
    {
        var strategy = new MaximumHorizontalScrollBarMaximumValueStrategy(new[] { OldDiffCodeTextBox.CodeTextBoxModel, NewDiffCodeTextBox.CodeTextBoxModel });
        OldDiffCodeTextBox.Viewport.SetHorizontalScrollBarMaximumValueStrategy(strategy);
        NewDiffCodeTextBox.Viewport.SetHorizontalScrollBarMaximumValueStrategy(strategy);
        OldDiffCodeTextBox.Viewport.UpdateScrollbarsMaximumValues();
        NewDiffCodeTextBox.Viewport.UpdateScrollbarsMaximumValues();
    }
}
