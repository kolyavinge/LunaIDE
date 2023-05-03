using CodeHighlighter.Model;
using Luna.ProjectModel;

namespace Luna.IDE.TextDiff;

public interface IDoubleTextDiff
{
    IDiffCodeTextBox OldDiffCodeTextBox { get; }

    IDiffCodeTextBox NewDiffCodeTextBox { get; }

    ILineNumberPanel OldLineNumberPanel { get; }

    ILineNumberPanel NewLineNumberPanel { get; }

    void MakeDiff(TextDiffResult diffResult, string? oldFileText, TextFileProjectItem newFile);
}
