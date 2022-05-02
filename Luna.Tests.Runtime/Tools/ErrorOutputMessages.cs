using System;
using System.Collections.Generic;
using Luna.Infrastructure;
using Luna.Parsing;

namespace Luna.Tests.Tools
{
    internal class ErrorOutputMessages : IOutputMessages
    {
        public void ConstDeclarationEmptyDeclaration(Token token) => throw new NotImplementedException();

        public void ConstDeclarationIncorrectName(Token token) => throw new NotImplementedException();

        public void ConstDeclarationIncorrectValue(Token token) => throw new NotImplementedException();

        public void ConstDeclarationNoValue(Token token) => throw new NotImplementedException();

        public void ImportDirectiveEmptyFilePath(Token token) => throw new NotImplementedException();

        public void ImportDirectiveFilePathNotString(Token token) => throw new NotImplementedException();

        public void ImportDirectiveIncorrectTokenAfter(Token token) => throw new NotImplementedException();

        public void ImportDirectiveNoFilePath(Token token) => throw new NotImplementedException();

        public void IncorrectToken(Token token) => throw new NotImplementedException();

        public void UnexpectedImportDirective(Token token) => throw new NotImplementedException();

        public void UnexpectedTokens(IEnumerable<Token> tokens) => throw new NotImplementedException();

        public void UnexpectedToken(Token token) => throw new NotImplementedException();

        public void IncorrectFunctionName(Token funcToken) => throw new NotImplementedException();

        public void UnexpectedFunctionEnd(Token funcToken) => throw new NotImplementedException();

        public void IncorrectFunctionAgrumentsDeclaration(Token token) => throw new NotImplementedException();

        public void IncorrectFunctionAgrument(Token token) => throw new NotImplementedException();

        public void IncorrectFunctionBody(Token token) => throw new NotImplementedException();

        public void UnknownIdentificator(Token token) => throw new NotImplementedException();
    }
}
