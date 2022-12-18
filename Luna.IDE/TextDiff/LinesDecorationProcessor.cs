using System.Collections.Generic;
using CodeHighlighter.Common;
using CodeHighlighter.Model;
using DiffTool.Core;
using DiffTool.Visualization;

namespace Luna.IDE.TextDiff;

public interface ILinesDecorationProcessor
{
    void SetLineColors(IReadOnlyList<SingleTextVisualizerLineDiff> linesDiff, ILinesDecorationCollection linesDecoration);
}

public class LinesDecorationProcessor : ILinesDecorationProcessor
{
    public static readonly Color BrushAdd = Color.FromHex("325a28");
    public static readonly Color BrushRemove = Color.FromHex("5a2828");

    public void SetLineColors(IReadOnlyList<SingleTextVisualizerLineDiff> linesDiff, ILinesDecorationCollection linesDecoration)
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
