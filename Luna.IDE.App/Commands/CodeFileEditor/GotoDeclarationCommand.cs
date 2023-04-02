using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.CodeEditing;
using Luna.Navigation;

namespace Luna.IDE.App.Commands.CodeFileEditor;

public interface IGotoDeclarationCommand : ICommand { }

public class GotoDeclarationCommand : Command<ICodeFileEditor>, IGotoDeclarationCommand
{
    private readonly ICodeModelNavigator _codeModelNavigator;
    private readonly IDeclarationNavigator _declarationNavigator;
    private readonly ICodeElementNavigateCommand _codeElementNavigateCommand;

    public GotoDeclarationCommand(
        ICodeModelNavigator codeModelNavigator,
        IDeclarationNavigator declarationNavigator,
        ICodeElementNavigateCommand codeElementNavigateCommand)
    {
        _codeModelNavigator = codeModelNavigator;
        _declarationNavigator = declarationNavigator;
        _codeElementNavigateCommand = codeElementNavigateCommand;
    }

    protected override void Execute(ICodeFileEditor codeFileEditor)
    {
        var position = codeFileEditor.GetTokenCursorPosition();
        if (position == null) return;
        CodeHighlighter.Core.Token token;
        if (position.NeighbourToken != null)
        {
            token = position.TokenOnPosition.IsIdentificator()
                ? position.TokenOnPosition
                : position.NeighbourToken;
        }
        else
        {
            token = position.TokenOnPosition;
        }
        var navigationResult = _codeModelNavigator.GetCodeElementByPosition(codeFileEditor.ProjectItem.CodeModel, codeFileEditor.CursorPosition.LineIndex, token.StartColumnIndex);
        if (navigationResult == null) return;
        var declarationResult = _declarationNavigator.GetDeclarationFor(codeFileEditor.ProjectItem, navigationResult.CodeElement);
        if (declarationResult == null) return;
        _codeElementNavigateCommand.Execute(new CodeElementNavigateCommandParameter(declarationResult.CodeFile, declarationResult.Declaration));
    }
}
