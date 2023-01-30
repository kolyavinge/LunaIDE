using System.Collections.Generic;
using System.Threading.Tasks;
using DiffTool.Core;
using DiffTool.Visualization;

namespace Luna.IDE.TextDiff;

public class TextDiffResult
{
    public Text OldText { get; }
    public Text NewText { get; }
    public IReadOnlyList<LineDiff> LinesDiff { get; }

    public TextDiffResult(Text oldText, Text newText, IReadOnlyList<LineDiff> linesDiff)
    {
        OldText = oldText;
        NewText = newText;
        LinesDiff = linesDiff;
    }
}

public class SingleTextDiffResult
{
    public int OldTextLinesCount { get; }
    public int NewTextLinesCount { get; }
    public SingleTextVisualizerResult VisualizerResult { get; }

    public SingleTextDiffResult(int oldTextLinesCount, int newTextLinesCount, SingleTextVisualizerResult visualizerResult)
    {
        OldTextLinesCount = oldTextLinesCount;
        NewTextLinesCount = newTextLinesCount;
        VisualizerResult = visualizerResult;
    }
}

public interface ITextDiffEngine
{
    Task<TextDiffResult> GetDiffResultAsync(string oldText, string newText);
    SingleTextDiffResult GetSingleTextResult(TextDiffResult diffResult);
}

public class TextDiffEngine : ITextDiffEngine
{
    public async Task<TextDiffResult> GetDiffResultAsync(string oldText, string newText)
    {
        return await Task.Factory.StartNew(() => GetDiffResult(oldText, newText));
    }

    private TextDiffResult GetDiffResult(string oldText, string newText)
    {
        var oldTextObj = new Text(oldText);
        var newTextObj = new Text(newText);
        var diffEngine = new DiffEngine();
        var diffResult = diffEngine.GetDiff(oldTextObj, newTextObj);

        return new TextDiffResult(oldTextObj, newTextObj, diffResult.LinesDiff);
    }

    public SingleTextDiffResult GetSingleTextResult(TextDiffResult diffResult)
    {
        var visualizator = new SingleTextVisualizer();
        var visualizationResult = visualizator.GetResult(diffResult.OldText, diffResult.NewText, diffResult.LinesDiff);

        return new(
            diffResult.OldText.IsEmpty ? 0 : diffResult.OldText.Lines.Count,
            diffResult.NewText.IsEmpty ? 0 : diffResult.NewText.Lines.Count,
            visualizationResult);
    }
}
