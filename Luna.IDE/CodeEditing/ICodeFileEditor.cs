using CodeHighlighter.Model;
using Luna.CodeElements;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

public interface ICodeFileEditor
{
    CodeFileProjectItem ProjectItem { get; }

    ICodeTextBoxModel CodeTextBoxModel { get; }

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

    void Close();

    void Save();
}
