using System.Collections.Generic;
using CodeHighlighter.Ancillary;
using DiffTool.Core;

namespace Luna.IDE.TextDiff;

internal interface IDoubleTextDiffGapProcessor
{
    void SetLineGaps(
        IReadOnlyList<LineDiff> lineDiffs,
        ILineGapCollection oldLineNumberPanelGaps,
        ILineGapCollection newLineNumberPanelGaps,
        ILineGapCollection oldDiffCodeTextBoxGaps,
        ILineGapCollection newDiffCodeTextBoxGaps);
}

internal class DoubleTextDiffGapProcessor : IDoubleTextDiffGapProcessor
{
    public void SetLineGaps(
        IReadOnlyList<LineDiff> lineDiffs,
        ILineGapCollection oldLineNumberPanelGaps,
        ILineGapCollection newLineNumberPanelGaps,
        ILineGapCollection oldDiffCodeTextBoxGaps,
        ILineGapCollection newDiffCodeTextBoxGaps)
    {
        int oldLineIndex = 0, newLineIndex = 0;
        foreach (var lineDiff in lineDiffs)
        {
            if (lineDiff.Kind == DiffKind.Add)
            {
                IncrementLineGap(oldLineNumberPanelGaps, oldLineIndex);
                IncrementLineGap(oldDiffCodeTextBoxGaps, oldLineIndex);
                newLineIndex++;
            }
            else if (lineDiff.Kind == DiffKind.Remove)
            {
                IncrementLineGap(newLineNumberPanelGaps, newLineIndex);
                IncrementLineGap(newDiffCodeTextBoxGaps, newLineIndex);
                oldLineIndex++;
            }
            else // Change and Same
            {
                newLineIndex++;
                oldLineIndex++;
            }
        }
    }

    private void IncrementLineGap(ILineGapCollection gaps, int lineIndex)
    {
        var gap = gaps[lineIndex];
        if (gap is not null)
        {
            gap.CountBefore++;
        }
        else
        {
            gaps[lineIndex] = new(1);
        }
    }
}
