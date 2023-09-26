using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Core;
using CodeHighlighter.Model;
using Luna.IDE.Common;
using Luna.ProjectModel;
using Luna.Utils;

namespace Luna.IDE.TextDiff;

internal class DoubleTextDiff : NotificationObject, IDoubleTextDiff
{
    private readonly ITextDiffCodeProviderFactory _textDiffCodeProviderFactory;
    private readonly ILinesDecorationProcessor _linesDecorationProcessor;
    private readonly IDoubleTextDiffGapProcessor _gapProcessor;
    private int _oldTextLinesCount;
    private int _newTextLinesCount;

    public IDiffCodeTextBox OldDiffCodeTextBox { get; }

    public IDiffCodeTextBox NewDiffCodeTextBox { get; }

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

    public DoubleTextDiff(
        ITextDiffCodeProviderFactory textDiffCodeProviderFactory,
        IDiffCodeTextBox oldDiffCodeTextBox,
        IDiffCodeTextBox newDiffCodeTextBox,
        ILineNumberPanel oldLineNumberPanel,
        ILineNumberPanel newLineNumberPanel,
        ILinesDecorationProcessor linesDecorationProcessor,
        IDoubleTextDiffGapProcessor gapProcessor)
    {
        _textDiffCodeProviderFactory = textDiffCodeProviderFactory;
        OldDiffCodeTextBox = oldDiffCodeTextBox;
        NewDiffCodeTextBox = newDiffCodeTextBox;
        OldLineNumberPanel = oldLineNumberPanel;
        NewLineNumberPanel = newLineNumberPanel;
        _linesDecorationProcessor = linesDecorationProcessor;
        _gapProcessor = gapProcessor;
    }

    public void MakeDiff(TextDiffResult diffResult, string? oldFileText, TextFileProjectItem newFile)
    {
        var codeProvider = _textDiffCodeProviderFactory.Make(oldFileText ?? "", newFile);
        InitDiffCodeTextBox(codeProvider, diffResult, oldFileText ?? "", newFile.GetText(), oldFileText is not null);
        InitGaps(diffResult);
        SynchronizeHorizontalScrollbars();
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
        var strategy = new MaximumHorizontalScrollBarMaximumValueStrategy(new[] { OldDiffCodeTextBox.CodeTextBox!, NewDiffCodeTextBox.CodeTextBox! });
        OldDiffCodeTextBox.Viewport.SetHorizontalScrollBarMaximumValueStrategy(strategy);
        NewDiffCodeTextBox.Viewport.SetHorizontalScrollBarMaximumValueStrategy(strategy);
        OldDiffCodeTextBox.Viewport.UpdateScrollBarsMaximumValues();
        NewDiffCodeTextBox.Viewport.UpdateScrollBarsMaximumValues();
    }
}
