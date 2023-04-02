using CodeHighlighter.Core;
using Luna.CodeElements;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

public interface ICodeFileEditor
{
    CodeFileProjectItem ProjectItem { get; }

    string Text { get; set; }

    CursorPosition CursorPosition { get; }

    IViewport Viewport { get; }

    ITextMeasures TextMeasures { get; }

    ITextEvents TextEvents { get; }

    ITokens Tokens { get; }

    TokenCursorPosition? GetTokenCursorPosition();

    void NavigateTo(CodeElement codeElement);

    void ReplaceText(CursorPosition start, CursorPosition end, string text);

    void UndoTextChanges();

    void DeleteSelectedLines();

    void ToLowerCase();

    void ToUpperCase();

    void MoveSelectedLinesUp();

    void MoveSelectedLinesDown();

    void MoveCursorTo(CursorPosition position);

    void Copy();

    void Paste();

    void Cut();

    void Undo();

    void Redo();

    void Close();

    void Save();
}
