using System.Collections.Generic;
using System.Windows.Media;
using CodeHighlighter.Model;
using DiffTool.Core;
using DiffTool.Visualization;
using Luna.IDE.Utils;

namespace Luna.IDE.TextDiff;

public interface ILinesDecorationProcessor
{
    void SetLineColors(IReadOnlyList<SingleTextVisualizerLineDiff> linesDiff, LinesDecorationCollection linesDecoration);
}

public class LinesDecorationProcessor : ILinesDecorationProcessor
{
    public static readonly SolidColorBrush BrushAdd = new(ColorUtils.FromHex("325a28"));
    public static readonly SolidColorBrush BrushRemove = new(ColorUtils.FromHex("5a2828"));

    public void SetLineColors(IReadOnlyList<SingleTextVisualizerLineDiff> linesDiff, LinesDecorationCollection linesDecoration)
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
