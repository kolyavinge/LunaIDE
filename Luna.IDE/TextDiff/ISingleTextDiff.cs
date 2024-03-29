﻿using CodeHighlighter.Model;
using Luna.ProjectModel;

namespace Luna.IDE.TextDiff;

public interface ISingleTextDiff
{
    IDiffCodeTextBox DiffCodeTextBox { get; }

    ILineNumberPanel OldLineNumberPanel { get; }

    ILineNumberPanel NewLineNumberPanel { get; }

    int OldTextLinesCount { get; }

    int NewTextLinesCount { get; }

    void MakeDiff(TextDiffResult diffResult, string fileExtension, string? oldFileText, string newFileText);

    void MakeDiff(TextDiffResult diffResult, string? oldFileText, TextFileProjectItem newFile);
}
