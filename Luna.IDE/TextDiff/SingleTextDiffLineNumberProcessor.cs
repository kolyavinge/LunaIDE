using System.Collections.Generic;
using CodeHighlighter.Model;
using DiffTool.Core;
using DiffTool.Visualization;

namespace Luna.IDE.TextDiff;

public interface ILineNumberProcessor
{
    void SetLineGaps(IReadOnlyList<SingleTextVisualizerLineDiff> linesDiff, LineNumberGapCollection oldGaps, LineNumberGapCollection newGaps);
}

public class SingleTextDiffLineNumberProcessor : ILineNumberProcessor
{
    public void SetLineGaps(IReadOnlyList<SingleTextVisualizerLineDiff> linesDiff, LineNumberGapCollection oldGaps, LineNumberGapCollection newGaps)
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

    private void IncrementLineGap(LineNumberGapCollection gaps, int lineIndex)
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
