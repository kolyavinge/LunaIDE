using System.Collections.Generic;
using System.Linq;
using Luna.Parsing;
using NUnit.Framework;

namespace Luna.Tests.Parsing
{
    public class ImportDirectiveParserTest
    {
        private CodeModel _codeModel;
        private ImportDirectiveParser _parser;
        private ParseResult _result;

        [SetUp]
        public void Setup()
        {
            _codeModel = new CodeModel();
        }

        [Test]
        public void ImportDirective_Correct()
        {
            Parse("import 'file' // comment", new[]
            {
                new Token(0, 0, 6, TokenKind.ImportDirective),
                new Token(0, 7, 6, TokenKind.String),
                new Token(0, 14, 10, TokenKind.Comment),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Imports.Count);
            Assert.AreEqual("file", _codeModel.Imports.First().FilePath);
            Assert.AreEqual(0, _codeModel.Imports.First().LineIndex);
            Assert.AreEqual(0, _codeModel.Imports.First().ColumnIndex);
        }

        [Test]
        public void ImportDirectives_Correct()
        {
            Parse("import 'file 1' // comment\r\nimport 'file 2'", new[]
            {
                new Token(0, 0, 6, TokenKind.ImportDirective),
                new Token(0, 7, 8, TokenKind.String),
                new Token(0, 14, 10, TokenKind.Comment),
                new Token(1, 0, 6, TokenKind.ImportDirective),
                new Token(1, 7, 8, TokenKind.String),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(2, _codeModel.Imports.Count);
            Assert.AreEqual("file 1", _codeModel.Imports.First().FilePath);
            Assert.AreEqual(0, _codeModel.Imports.First().LineIndex);
            Assert.AreEqual(0, _codeModel.Imports.First().ColumnIndex);
            Assert.AreEqual("file 2", _codeModel.Imports.Last().FilePath);
            Assert.AreEqual(1, _codeModel.Imports.Last().LineIndex);
            Assert.AreEqual(0, _codeModel.Imports.Last().ColumnIndex);
        }

        [Test]
        public void ImportDirective_EmptyFilePath_Error()
        {
            Parse("import ''", new[]
            {
                new Token(0, 0, 6, TokenKind.ImportDirective),
                new Token(0, 7, 2, TokenKind.String)
            });
            Assert.AreEqual(0, _codeModel.Imports.Count);
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.ImportEmptyFilePath, _result.Error.Type);
            Assert.AreEqual(new Token(0, 7, 2, TokenKind.String), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ImportDirective_NoFilePath_Error()
        {
            Parse("import ", new[]
            {
                new Token(0, 0, 6, TokenKind.ImportDirective)
            });
            Assert.AreEqual(0, _codeModel.Imports.Count);
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.ImportNoFilePath, _result.Error.Type);
            Assert.AreEqual(new Token(0, 0, 6, TokenKind.ImportDirective), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ImportDirective_NoFilePath2_Error()
        {
            Parse("import\r\nimport 'file'", new[]
            {
                new Token(0, 0, 6, TokenKind.ImportDirective),
                new Token(1, 0, 6, TokenKind.ImportDirective),
                new Token(1, 7, 2, TokenKind.String),
            });
            Assert.AreEqual(0, _codeModel.Imports.Count);
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.ImportNoFilePath, _result.Error.Type);
            Assert.AreEqual(new Token(0, 0, 6, TokenKind.ImportDirective), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ImportDirective_IntegerFilePath_Error()
        {
            Parse("import 123", new[]
            {
                new Token(0, 0, 6, TokenKind.ImportDirective),
                new Token(0, 7, 3, TokenKind.IntegerNumber),
            });
            Assert.AreEqual(0, _codeModel.Imports.Count);
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.ImportFilePathNotString, _result.Error.Type);
            Assert.AreEqual(new Token(0, 7, 3, TokenKind.IntegerNumber), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ImportDirective_FloatFilePath_Error()
        {
            Parse("import 1.23", new[]
            {
                new Token(0, 0, 6, TokenKind.ImportDirective),
                new Token(0, 7, 4, TokenKind.FloatNumber),
            });
            Assert.AreEqual(0, _codeModel.Imports.Count);
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.ImportFilePathNotString, _result.Error.Type);
            Assert.AreEqual(new Token(0, 7, 4, TokenKind.FloatNumber), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ImportDirective_Incorrect_Error()
        {
            Parse("import import", new[]
            {
                new Token(0, 0, 6, TokenKind.ImportDirective),
                new Token(0, 7, 6, TokenKind.ImportDirective),
            });
            Assert.AreEqual(0, _codeModel.Imports.Count);
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.ImportIncorrectTokenAfter, _result.Error.Type);
            Assert.AreEqual(new Token(0, 7, 6, TokenKind.ImportDirective), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ImportDirective_UnexpectedTokens()
        {
            Parse("import 'file' 1 1.2 'string'", new[]
            {
                new Token(0, 0, 6, TokenKind.ImportDirective),
                new Token(0, 7, 6, TokenKind.String),
                new Token(0, 14, 1, TokenKind.IntegerNumber),
                new Token(0, 16, 3, TokenKind.FloatNumber),
                new Token(0, 20, 8, TokenKind.String),
            });
            Assert.AreEqual(0, _codeModel.Imports.Count);
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.UnexpectedToken, _result.Error.Type);
            Assert.AreEqual(new Token(0, 14, 1, TokenKind.IntegerNumber), _result.Error.Tokens[0]);
            Assert.AreEqual(new Token(0, 16, 3, TokenKind.FloatNumber), _result.Error.Tokens[1]);
            Assert.AreEqual(new Token(0, 20, 8, TokenKind.String), _result.Error.Tokens[2]);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ImportDirective_UnexpectedTokens2()
        {
            Parse("import 'file' import 'file'", new[]
            {
                new Token(0, 0, 6, TokenKind.ImportDirective),
                new Token(0, 7, 6, TokenKind.String),
                new Token(0, 14, 6, TokenKind.ImportDirective),
                new Token(0, 21, 8, TokenKind.String),
            });
            Assert.AreEqual(0, _codeModel.Imports.Count);
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.UnexpectedToken, _result.Error.Type);
            Assert.AreEqual(new Token(0, 14, 6, TokenKind.ImportDirective), _result.Error.Tokens[0]);
            Assert.AreEqual(new Token(0, 21, 8, TokenKind.String), _result.Error.Tokens[1]);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        private void Parse(string text, IEnumerable<Token> tokens)
        {
            _parser = new ImportDirectiveParser(new Text(text), new TokenIterator(tokens), _codeModel);
            _result = _parser.Parse();
        }
    }
}
