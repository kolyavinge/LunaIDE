using System.Collections.Generic;
using Luna.Parsing;

namespace Luna.Infrastructure
{
    public interface IOutputMessages
    {
        void IncorrectToken(Token token);
        void UnexpectedTokens(IEnumerable<Token> tokens);
        void UnexpectedToken(Token token);
        void ImportDirectiveIncorrectTokenAfter(Token token);
        void ImportDirectiveEmptyFilePath(Token token);
        void ImportDirectiveNoFilePath(Token token);
        void ImportDirectiveFilePathNotString(Token token);
        void ConstDeclarationEmptyDeclaration(Token token);
        void ConstDeclarationIncorrectName(Token token);
        void ConstDeclarationNoValue(Token token);
        void ConstDeclarationIncorrectValue(Token token);
        void UnexpectedImportDirective(Token token);
        void IncorrectFunctionName(Token funcToken);
        void UnexpectedFunctionEnd(Token funcToken);
        void IncorrectFunctionAgrumentsDeclaration(Token token);
        void IncorrectFunctionAgrument(Token token);
        void IncorrectFunctionBody(Token token);
        void UnknownIdentificator(Token token);
    }

    public class OutputBuilder
    {
    }
}
