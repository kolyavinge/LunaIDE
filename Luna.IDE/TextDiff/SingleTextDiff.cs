using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using CodeHighlighter.Model;
using CodeHighlighter.Rendering;
using DiffTool.Core;
using DiffTool.Visualization;
using Luna.IDE.Common;
using Luna.IDE.Utils;

namespace Luna.IDE.TextDiff;

public interface ISingleTextDiff
{
    CodeTextBoxModel? DiffCodeTextBoxModel { get; }
    bool InProgress { get; }
    Task MakeDiff(string fileExtension, string? oldFileText, string newFileText);
}

public class SingleTextDiff : NotificationObject, ISingleTextDiff
{
    public static readonly SolidColorBrush BrushAdd = new(ColorUtils.FromHex("325a28"));
    public static readonly SolidColorBrush BrushRemove = new(ColorUtils.FromHex("5a2828"));

    private readonly ITextDiffEngine _textDiffEngine;
    private readonly ITextDiffCodeProviderFactory _textDiffCodeProviderFactory;
    private CodeTextBoxModel? _diffCodeTextBoxModel;
    private bool _inProgress;

    public CodeTextBoxModel? DiffCodeTextBoxModel
    {
        get => _diffCodeTextBoxModel;
        private set { _diffCodeTextBoxModel = value; RaisePropertyChanged(() => DiffCodeTextBoxModel!); }
    }

    public bool InProgress
    {
        get => _inProgress;
        set { _inProgress = value; RaisePropertyChanged(() => InProgress); }
    }

    public SingleTextDiff(ITextDiffEngine textDiffEngine, ITextDiffCodeProviderFactory textDiffCodeProviderFactory)
    {
        _textDiffEngine = textDiffEngine;
        _textDiffCodeProviderFactory = textDiffCodeProviderFactory;
    }

    public async Task MakeDiff(string fileExtension, string? oldFileText, string newFileText)
    {
        InProgress = true;
        var diffResult = await _textDiffEngine.GetSingleTextResultAsync(oldFileText ?? "", newFileText);
        var codeProvider = _textDiffCodeProviderFactory.Make(fileExtension, oldFileText ?? "", newFileText);
        DiffCodeTextBoxModel = new CodeTextBoxModel(codeProvider, new() { HighlighteredBrackets = "()" });
        DiffCodeTextBoxModel.IsReadOnly = false;
        DiffCodeTextBoxModel.SetText(diffResult.Text);
        DiffCodeTextBoxModel.IsReadOnly = true;
        DiffCodeTextBoxModel.LinesDecoration.Clear();
        if (oldFileText != null)
        {
            SetLineColors(diffResult.LinesDiff, DiffCodeTextBoxModel.LinesDecoration);
        }
        InProgress = false;
    }

    private void SetLineColors(IReadOnlyList<SingleTextVisualizerLineDiff> linesDiff, LinesDecorationCollection linesDecoration)
    {
        for (int i = 0; i < linesDiff.Count; i++)
        {
            var lineDiff = linesDiff[i];
            if (lineDiff.DiffKind == DiffKind.Add)
            {
                linesDecoration[i] = new() { Background = BrushAdd };
            }
            else if (lineDiff.DiffKind == DiffKind.Remove)
            {
                linesDecoration[i] = new() { Background = BrushRemove };
            }
            else if (lineDiff.DiffKind == DiffKind.Change && lineDiff.TextKind == TextKind.Old)
            {
                linesDecoration[i] = new() { Background = BrushRemove };
            }
            else if (lineDiff.DiffKind == DiffKind.Change && lineDiff.TextKind == TextKind.New)
            {
                linesDecoration[i] = new() { Background = BrushAdd };
            }
        }
    }
}
