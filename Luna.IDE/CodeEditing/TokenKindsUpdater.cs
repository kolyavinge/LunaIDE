using System.Linq;
using CodeHighlighter.CodeProvidering;
using Luna.Parsing;
using Luna.ProjectModel;

namespace Luna.IDE.CodeEditing;

public interface ITokenKindsUpdater
{
    void Update(CodeModelScopeIdentificatorsDifferent diff, ILunaCodeProvider codeProvider);
}

public class TokenKindsUpdater : ITokenKindsUpdater
{
    public void Update(CodeModelScopeIdentificatorsDifferent diff, ILunaCodeProvider codeProvider)
    {
        var updatedTokens =
            diff.AddedDeclaredConstants.Concat(diff.AddedImportedConstants).Select(x => new UpdatedTokenKind(x.Name, (byte)TokenKindExtra.Constant)).
            Concat(diff.AddedDeclaredFunctions.Concat(diff.AddedImportedFunctions).Select(x => new UpdatedTokenKind(x.Name, (byte)TokenKindExtra.Function))).
            Concat(diff.RemovedDeclaredConstants.Concat(diff.RemovedImportedConstants).Select(x => new UpdatedTokenKind(x.Name, (byte)TokenKind.Identificator))).
            Concat(diff.RemovedDeclaredFunctions.Concat(diff.RemovedImportedFunctions).Select(x => new UpdatedTokenKind(x.Name, (byte)TokenKind.Identificator))).
            ToList();

        if (updatedTokens.Any())
        {
            codeProvider.UpdateTokenKinds(updatedTokens);
        }
    }
}
