using CodeHighlighter.Model;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

public interface ICodeFileEditor
{
    CodeFileProjectItem ProjectItem { get; }

    CodeTextBoxModel CodeTextBoxModel { get; }

    CursorPosition CursorPosition { get; }

    TokenCursorPosition? GetTokenCursorPosition();

    void NavigateTo(CodeElement codeElement);

    void ReplaceText(CursorPosition start, CursorPosition end, string text);

    void UndoTextChanges();

    void DeleteSelectedLines();

    void ToLowerCase();

    void ToUpperCase();

    void MoveSelectedLinesUp();

    void MoveSelectedLinesDown();

    void Undo();

    void Redo();
}
