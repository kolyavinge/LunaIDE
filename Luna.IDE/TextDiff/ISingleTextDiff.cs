using System.Threading.Tasks;
using CodeHighlighter.Model;
using Luna.ProjectModel;

namespace Luna.IDE.TextDiff;

public interface ISingleTextDiff
{
    CodeTextBoxModel DiffCodeTextBox { get; }

    LineNumberPanelModel OldLineNumberPanel { get; }

    LineNumberPanelModel NewLineNumberPanel { get; }

    int OldTextLinesCount { get; }

    int NewTextLinesCount { get; }

    bool InProgress { get; }

    Task MakeDiff(string fileExtension, string? oldFileText, string newFileText);

    Task MakeDiff(string? oldFileText, TextFileProjectItem newFile);
}
