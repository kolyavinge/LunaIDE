using System.Collections.Generic;
using CodeHighlighter.Ancillary;
using DiffTool.Core;
using DiffTool.Visualization;

namespace Luna.IDE.TextDiff;

internal interface ISingleTextDiffGapProcessor
{
    void SetLineGaps(IReadOnlyList<SingleTextVisualizerLineDiff> linesDiff, ILineGapCollection oldGaps, ILineGapCollection newGaps);
}

internal class SingleTextDiffGapProcessor : ISingleTextDiffGapProcessor
{
    public void SetLineGaps(IReadOnlyList<SingleTextVisualizerLineDiff> linesDiff, ILineGapCollection oldGaps, ILineGapCollection newGaps)
    {
        int oldLineIndex = 0, newLineIndex = 0;
        foreach (var lineDiff in linesDiff)
        {
            if (lineDiff.DiffKind == DiffKind.Add)
            {
                IncrementLineGap(oldGaps, oldLineIndex);
                newLineIndex++;
            }
            else if (lineDiff.DiffKind == DiffKind.Remove)
            {
                IncrementLineGap(newGaps, newLineIndex);
                oldLineIndex++;
            }
            else if (lineDiff.DiffKind == DiffKind.Change && lineDiff.TextKind == TextKind.Old)
            {
                IncrementLineGap(newGaps, newLineIndex);
                oldLineIndex++;
            }
            else if (lineDiff.DiffKind == DiffKind.Change && lineDiff.TextKind == TextKind.New)
            {
                IncrementLineGap(oldGaps, oldLineIndex);
                newLineIndex++;
            }
            else // Same
            {
                oldLineIndex++;
                newLineIndex++;
            }
        }
    }

    private void IncrementLineGap(ILineGapCollection gaps, int lineIndex)
    {
        var gap = gaps[lineIndex];
        if (gap != null)
        {
            gap.CountBefore++;
        }
        else
        {
            gaps[lineIndex] = new(1);
        }
    }
}
