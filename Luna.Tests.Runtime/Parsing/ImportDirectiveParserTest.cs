using System.Collections.Generic;
using System.Linq;
using Luna.Parsing;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Parsing;

internal class ImportDirectiveParserTest
{
    private Mock<IImportDirectiveParserScope> _scope;
    private CodeModel _codeModel;
    private ImportDirectiveParser _parser;
    private ParseResult _result;

    [SetUp]
    public void Setup()
    {
        _scope = new Mock<IImportDirectiveParserScope>();
        _scope.Setup(x => x.GetCodeFileByPath("file")).Returns(new CodeFileProjectItem("", null, null));
        _scope.Setup(x => x.GetCodeFileByPath("file 1")).Returns(new CodeFileProjectItem("", null, null));
        _scope.Setup(x => x.GetCodeFileByPath("file 2")).Returns(new CodeFileProjectItem("", null, null));
        _codeModel = new CodeModel();
    }

    [Test]
    public void ImportDirective_Correct()
    {
        // import 'file' // comment
        Parse(new Token[]
        {
            new("import", 0, 0, 6, TokenKind.ImportDirective),
            new("'file'", 0, 7, 6, TokenKind.String),
            new("// comment", 0, 14, 10, TokenKind.Comment),
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
        // import 'file 1' // comment\r\nimport 'file 2'
        Parse(new Token[]
        {
            new("import", 0, 0, 6, TokenKind.ImportDirective),
            new("'file 1'", 0, 7, 8, TokenKind.String),
            new("// comment", 0, 14, 10, TokenKind.Comment),
            new("import", 1, 0, 6, TokenKind.ImportDirective),
            new("'file 2'", 1, 7, 8, TokenKind.String),
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
        // import ''
        Parse(new Token[]
        {
            new("import", 0, 0, 6, TokenKind.ImportDirective),
            new("''", 0, 7, 2, TokenKind.String)
        });
        Assert.AreEqual(0, _codeModel.Imports.Count);
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.NotNull(_result.Error);
        Assert.AreEqual(ParserMessageType.ImportEmptyFilePath, _result.Error.Type);
        Assert.AreEqual(new Token("''", 0, 7, 2, TokenKind.String), _result.Error.Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ImportDirective_NoFilePath_Error()
    {
        // import 
        Parse(new Token[]
        {
            new("import", 0, 0, 6, TokenKind.ImportDirective)
        });
        Assert.AreEqual(0, _codeModel.Imports.Count);
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.NotNull(_result.Error);
        Assert.AreEqual(ParserMessageType.ImportNoFilePath, _result.Error.Type);
        Assert.AreEqual(new Token("import", 0, 0, 6, TokenKind.ImportDirective), _result.Error.Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ImportDirective_NoFilePath2_Error()
    {
        // import\r\nimport 'file'
        Parse(new Token[]
        {
            new("import", 0, 0, 6, TokenKind.ImportDirective),
            new("import", 1, 0, 6, TokenKind.ImportDirective),
            new("'file'", 1, 7, 2, TokenKind.String),
        });
        Assert.AreEqual(0, _codeModel.Imports.Count);
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.NotNull(_result.Error);
        Assert.AreEqual(ParserMessageType.ImportNoFilePath, _result.Error.Type);
        Assert.AreEqual(new Token("import", 0, 0, 6, TokenKind.ImportDirective), _result.Error.Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ImportDirective_FileNotFound()
    {
        // import 'wrong'
        Parse(new Token[]
        {
            new("import", 0, 0, 6, TokenKind.ImportDirective),
            new("'wrong'", 0, 7, 7, TokenKind.String),
        });
        Assert.NotNull(_result.Error);
        Assert.AreEqual(ParserMessageType.ImportFileNotFound, _result.Error.Type);
        Assert.AreEqual(new Token("import", 0, 0, 6, TokenKind.ImportDirective), _result.Error.Token);
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(0, _codeModel.Imports.Count);
    }

    [Test]
    public void ImportDirective_IntegerFilePath_Error()
    {
        // import 123
        Parse(new Token[]
        {
            new("import", 0, 0, 6, TokenKind.ImportDirective),
            new("123", 0, 7, 3, TokenKind.IntegerNumber),
        });
        Assert.AreEqual(0, _codeModel.Imports.Count);
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.NotNull(_result.Error);
        Assert.AreEqual(ParserMessageType.ImportFilePathNotString, _result.Error.Type);
        Assert.AreEqual(new Token("123", 0, 7, 3, TokenKind.IntegerNumber), _result.Error.Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ImportDirective_FloatFilePath_Error()
    {
        // import 1.23
        Parse(new Token[]
        {
            new("import", 0, 0, 6, TokenKind.ImportDirective),
            new("1.23", 0, 7, 4, TokenKind.FloatNumber),
        });
        Assert.AreEqual(0, _codeModel.Imports.Count);
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.NotNull(_result.Error);
        Assert.AreEqual(ParserMessageType.ImportFilePathNotString, _result.Error.Type);
        Assert.AreEqual(new Token("1.23", 0, 7, 4, TokenKind.FloatNumber), _result.Error.Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ImportDirective_Incorrect_Error()
    {
        // import import
        Parse(new Token[]
        {
            new("import", 0, 0, 6, TokenKind.ImportDirective),
            new("import", 0, 7, 6, TokenKind.ImportDirective),
        });
        Assert.AreEqual(0, _codeModel.Imports.Count);
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.NotNull(_result.Error);
        Assert.AreEqual(ParserMessageType.IncorrectTokenAfterImport, _result.Error.Type);
        Assert.AreEqual(new Token("import", 0, 7, 6, TokenKind.ImportDirective), _result.Error.Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ImportDirective_UnexpectedTokens()
    {
        // import 'file' 1 1.2 'string'
        Parse(new Token[]
        {
            new("import", 0, 0, 6, TokenKind.ImportDirective),
            new("'file'", 0, 7, 6, TokenKind.String),
            new("1", 0, 14, 1, TokenKind.IntegerNumber),
            new("1.2", 0, 16, 3, TokenKind.FloatNumber),
            new("'string'", 0, 20, 8, TokenKind.String),
        });
        Assert.AreEqual(0, _codeModel.Imports.Count);
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.NotNull(_result.Error);
        Assert.AreEqual(ParserMessageType.UnexpectedToken, _result.Error.Type);
        Assert.AreEqual(new Token("1", 0, 14, 1, TokenKind.IntegerNumber), _result.Error.Tokens[0]);
        Assert.AreEqual(new Token("1.2", 0, 16, 3, TokenKind.FloatNumber), _result.Error.Tokens[1]);
        Assert.AreEqual(new Token("'string'", 0, 20, 8, TokenKind.String), _result.Error.Tokens[2]);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ImportDirective_UnexpectedTokens2()
    {
        // import 'file' import 'file'
        Parse(new Token[]
        {
            new("import", 0, 0, 6, TokenKind.ImportDirective),
            new("'file'", 0, 7, 6, TokenKind.String),
            new("import", 0, 14, 6, TokenKind.ImportDirective),
            new("'file'", 0, 21, 8, TokenKind.String),
        });
        Assert.AreEqual(0, _codeModel.Imports.Count);
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.NotNull(_result.Error);
        Assert.AreEqual(ParserMessageType.UnexpectedToken, _result.Error.Type);
        Assert.AreEqual(new Token("import", 0, 14, 6, TokenKind.ImportDirective), _result.Error.Tokens[0]);
        Assert.AreEqual(new Token("'file'", 0, 21, 8, TokenKind.String), _result.Error.Tokens[1]);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    private void Parse(IEnumerable<Token> tokens)
    {
        _parser = new ImportDirectiveParser(new TokenIterator(tokens), _codeModel, _scope.Object);
        _result = _parser.Parse();
    }
}
