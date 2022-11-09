using System.Threading.Tasks;
using DiffTool.Core;
using DiffTool.Visualization;

namespace Luna.IDE.TextDiff;

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
    SingleTextDiffResult GetSingleTextResult(string oldText, string newText);
    Task<SingleTextDiffResult> GetSingleTextResultAsync(string oldText, string newText);
}

public class TextDiffEngine : ITextDiffEngine
{
    public SingleTextDiffResult GetSingleTextResult(string oldText, string newText)
    {
        var oldTextObj = new Text(oldText);
        var newTextObj = new Text(newText);
        var diffEngine = new DiffEngine();
        var diffResult = diffEngine.GetDiff(oldTextObj, newTextObj);
        var visualizator = new SingleTextVisualizer();
        var visualizationResult = visualizator.GetResult(oldTextObj, newTextObj, diffResult.LinesDiff);

        return new(
            oldTextObj.IsEmpty ? 0 : oldTextObj.Lines.Count,
            newTextObj.IsEmpty ? 0 : newTextObj.Lines.Count,
            visualizationResult);
    }

    public async Task<SingleTextDiffResult> GetSingleTextResultAsync(string oldText, string newText)
    {
        return await Task.Factory.StartNew(() => GetSingleTextResult(oldText, newText));
    }
}
