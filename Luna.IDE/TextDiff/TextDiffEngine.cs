using System.Threading.Tasks;
using DiffTool.Core;
using DiffTool.Visualization;

namespace Luna.IDE.TextDiff;

public interface ITextDiffEngine
{
    SingleTextVisualizerResult GetSingleTextResult(string oldText, string newText);
    Task<SingleTextVisualizerResult> GetSingleTextResultAsync(string oldText, string newText);
}

public class TextDiffEngine : ITextDiffEngine
{
    public SingleTextVisualizerResult GetSingleTextResult(string oldText, string newText)
    {
        var oldTextObj = new Text(oldText);
        var newTextObj = new Text(newText);
        var diffEngine = new DiffEngine();
        var diffResult = diffEngine.GetDiff(oldTextObj, newTextObj);
        var visualizator = new SingleTextVisualizer();
        var visualizationResult = visualizator.GetResult(oldTextObj, newTextObj, diffResult.LinesDiff);

        return visualizationResult;
    }

    public async Task<SingleTextVisualizerResult> GetSingleTextResultAsync(string oldText, string newText)
    {
        return await Task.Factory.StartNew(() => GetSingleTextResult(oldText, newText));
    }
}
