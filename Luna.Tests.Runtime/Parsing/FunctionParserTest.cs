using System.Collections.Generic;
using System.Linq;
using Luna.Parsing;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Parsing;

internal class FunctionParserTest
{
    private Mock<IFunctionParserScope> _scopeMock;
    private CodeModel _codeModel;
    private FunctionParser _parser;
    private ParseResult _result;

    [SetUp]
    public void Setup()
    {
        _scopeMock = new Mock<IFunctionParserScope>();
        _scopeMock.Setup(x => x.IsConstantExist(It.IsAny<string>())).Returns(false);
        _scopeMock.Setup(x => x.IsFunctionExist(It.IsAny<string>())).Returns(false);
        _codeModel = new CodeModel();
    }

    [Test]
    public void ConstDeclaration_Correct_Integer()
    {
        // const WIDTH 123 // comment
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, 5, TokenKind.Identificator),
            new("123", 0, 12, 3, TokenKind.IntegerNumber),
            new("// comment", 0, 16, 10, TokenKind.Comment)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Constants.Count);
        Assert.AreEqual("WIDTH", _codeModel.Constants.First().Name);
        Assert.AreEqual(123, ((IntegerValueElement)_codeModel.Constants.First().Value).Value);
        Assert.AreEqual(0, _codeModel.Constants.First().LineIndex);
        Assert.AreEqual(6, _codeModel.Constants.First().ColumnIndex);
    }

    [Test]
    public void ConstDeclaration_IntegerValueOverflow()
    {
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, 5, TokenKind.Identificator),
            new("11111111111111111111111111111111111111111111111111", 0, 12, 50, TokenKind.IntegerNumber)
        });
        Assert.AreEqual(1, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IntegerValueOverflow, _result.Errors.First().Type);
        Assert.AreEqual(new Token("11111111111111111111111111111111111111111111111111", 0, 12, 50, TokenKind.IntegerNumber), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ConstDeclaration_Correct_Float()
    {
        // const WIDTH 1.23 // comment
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, 5, TokenKind.Identificator),
            new("1.23", 0, 12, 4, TokenKind.FloatNumber),
            new("// comment", 0, 17, 10, TokenKind.Comment)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Constants.Count);
        Assert.AreEqual("WIDTH", _codeModel.Constants.First().Name);
        Assert.AreEqual(typeof(FloatValueElement), _codeModel.Constants.First().Value.GetType());
        Assert.AreEqual(1.23, ((FloatValueElement)_codeModel.Constants.First().Value).Value);
        Assert.AreEqual(0, _codeModel.Constants.First().LineIndex);
        Assert.AreEqual(6, _codeModel.Constants.First().ColumnIndex);
    }

    [Test]
    public void ConstDeclaration_Correct_BooleanTrue()
    {
        // const TRUE true // comment
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("TRUE", 0, 6, 4, TokenKind.Identificator),
            new("true", 0, 11, 4, TokenKind.BooleanTrue),
            new("// comment", 0, 15, 10, TokenKind.Comment)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Constants.Count);
        Assert.AreEqual("TRUE", _codeModel.Constants.First().Name);
        Assert.AreEqual(typeof(BooleanValueElement), _codeModel.Constants.First().Value.GetType());
        Assert.AreEqual(true, ((BooleanValueElement)_codeModel.Constants.First().Value).Value);
        Assert.AreEqual(0, _codeModel.Constants.First().LineIndex);
        Assert.AreEqual(6, _codeModel.Constants.First().ColumnIndex);
    }

    [Test]
    public void ConstDeclaration_Correct_BooleanFalse()
    {
        // const FALSE false // comment
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("FALSE", 0, 6, 5, TokenKind.Identificator),
            new("false", 0, 12, 5, TokenKind.BooleanFalse),
            new("// comment", 0, 17, 10, TokenKind.Comment)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Constants.Count);
        Assert.AreEqual("FALSE", _codeModel.Constants.First().Name);
        Assert.AreEqual(typeof(BooleanValueElement), _codeModel.Constants.First().Value.GetType());
        Assert.AreEqual(false, ((BooleanValueElement)_codeModel.Constants.First().Value).Value);
        Assert.AreEqual(0, _codeModel.Constants.First().LineIndex);
        Assert.AreEqual(6, _codeModel.Constants.First().ColumnIndex);
    }

    [Test]
    public void ConstDeclaration_Correct_String()
    {
        // const WIDTH '123' // comment
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, 5, TokenKind.Identificator),
            new("'123'", 0, 12, 5, TokenKind.String),
            new("// comment", 0, 18, 10, TokenKind.Comment)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Constants.Count);
        Assert.AreEqual("WIDTH", _codeModel.Constants.First().Name);
        Assert.AreEqual("123", ((StringValueElement)_codeModel.Constants.First().Value).Value);
        Assert.AreEqual(0, _codeModel.Constants.First().LineIndex);
        Assert.AreEqual(6, _codeModel.Constants.First().ColumnIndex);
    }

    [Test]
    public void ConstDeclaration_Correct_2()
    {
        // const WIDTH 123\r\nconst VALUE 456
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, 5, TokenKind.Identificator),
            new("123", 0, 12, 3, TokenKind.IntegerNumber),
            new("const", 1, 0, 5, TokenKind.ConstDeclaration),
            new("VALUE", 1, 6, 5, TokenKind.Identificator),
            new("456", 1, 12, 3, TokenKind.IntegerNumber)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(2, _codeModel.Constants.Count);
    }

    [Test]
    public void ConstDeclaration_ConstNameExist()
    {
        // const WIDTH 123 // comment
        _scopeMock.Setup(x => x.IsConstantExist("WIDTH")).Returns(true);
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, 5, TokenKind.Identificator),
            new("123", 0, 12, 3, TokenKind.IntegerNumber),
            new("// comment", 0, 16, 10, TokenKind.Comment)
        });
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.ConstNameExist, _result.Errors.First().Type);
        Assert.AreEqual(new Token("WIDTH", 0, 6, 5, TokenKind.Identificator), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ConstDeclaration_FunctionNameExist()
    {
        // const WIDTH 123 // comment
        _scopeMock.Setup(x => x.IsFunctionExist("WIDTH")).Returns(true);
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, 5, TokenKind.Identificator),
            new("123", 0, 12, 3, TokenKind.IntegerNumber),
            new("// comment", 0, 16, 10, TokenKind.Comment)
        });
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.FunctionNameExist, _result.Errors.First().Type);
        Assert.AreEqual(new Token("WIDTH", 0, 6, 5, TokenKind.Identificator), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ConstDeclaration_EmptyDeclaration()
    {
        // const 
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration)
        });
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.EmptyConstDeclaration, _result.Errors.First().Type);
        Assert.AreEqual(new Token("const", 0, 0, 5, TokenKind.ConstDeclaration), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ConstDeclaration_IncorrectName_String()
    {
        // const '123'
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("'123'", 0, 6, 5, TokenKind.String)
        });
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectConstName, _result.Errors.First().Type);
        Assert.AreEqual(new Token("'123'", 0, 6, 5, TokenKind.String), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ConstDeclaration_IncorrectName_Integer()
    {
        // const 123
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("123", 0, 6, 3, TokenKind.IntegerNumber)
        });
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectConstName, _result.Errors.First().Type);
        Assert.AreEqual(new Token("123", 0, 6, 3, TokenKind.IntegerNumber), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ConstDeclaration_IncorrectName_Float()
    {
        // const 1.23
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("1.23", 0, 6, 4, TokenKind.FloatNumber)
        });
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectConstName, _result.Errors.First().Type);
        Assert.AreEqual(new Token("1.23", 0, 6, 4, TokenKind.FloatNumber), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ConstDeclaration_NoValue()
    {
        // const WIDTH
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, 5, TokenKind.Identificator)
        });
        Assert.AreEqual(1, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.ConstNoValue, _result.Errors.First().Type);
        Assert.AreEqual(new Token("const", 0, 0, 5, TokenKind.ConstDeclaration), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ConstDeclaration_IncorrectValue()
    {
        // const WIDTH VALUE
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, 5, TokenKind.Identificator),
            new("VALUE", 0, 12, 5, TokenKind.Identificator)
        });
        Assert.AreEqual(1, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.ConstIncorrectValue, _result.Errors.First().Type);
        Assert.AreEqual(new Token("VALUE", 0, 12, 5, TokenKind.Identificator), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ConstDeclaration_UnexpectedTokens()
    {
        // const WIDTH 123 const VALUE 456
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, 5, TokenKind.Identificator),
            new("123", 0, 12, 3, TokenKind.IntegerNumber),
            new("const", 0, 16, 5, TokenKind.ConstDeclaration),
            new("VALUE", 0, 22, 5, TokenKind.Identificator),
            new("456", 0, 28, 3, TokenKind.IntegerNumber)
        });
        Assert.AreEqual(0, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.UnexpectedToken, _result.Errors.First().Type);
        Assert.AreEqual(new Token("const", 0, 16, 5, TokenKind.ConstDeclaration), _result.Errors.First().Tokens[0]);
        Assert.AreEqual(new Token("VALUE", 0, 22, 5, TokenKind.Identificator), _result.Errors.First().Tokens[1]);
        Assert.AreEqual(new Token("456", 0, 28, 3, TokenKind.IntegerNumber), _result.Errors.First().Tokens[2]);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ConstDeclaration_UnexpectedImportDirective()
    {
        // const WIDTH '123'\r\nimport 'file'
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, 5, TokenKind.Identificator),
            new("'123'", 0, 12, 5, TokenKind.String),
            new("import", 1, 0, 6, TokenKind.ImportDirective),
            new("'file'", 1, 7, 6, TokenKind.String)
        });
        Assert.AreEqual(0, _codeModel.Imports.Count);
        Assert.AreEqual(1, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.UnexpectedImport, _result.Errors.First().Type);
        Assert.AreEqual(new Token("import", 1, 0, 6, TokenKind.ImportDirective), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ConstDeclaration_FirstConstIncorrect()
    {
        // const WIDTH 1x
        // const HEIGHT 2
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, 5, TokenKind.Identificator),
            new("1x", 0, 12, 2, TokenKind.Unknown),
            new("const", 1, 0, 5, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 6, 6, TokenKind.Identificator),
            new("2", 1, 13, 1, TokenKind.IntegerNumber)
        });
        Assert.AreEqual(0, _codeModel.Imports.Count);
        Assert.AreEqual(2, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.ConstIncorrectValue, _result.Errors.First().Type);
        Assert.AreEqual(new Token("1x", 0, 12, 2, TokenKind.Unknown), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ConstDeclaration_FirstConstNoValue()
    {
        // const WIDTH // comment
        // const HEIGHT 2
        Parse(new Token[]
        {
            new("const", 0, 0, 5, TokenKind.ConstDeclaration),
            new("WIDTH", 0, 6, 5, TokenKind.Identificator),
            new("// comment", 0, 13, 10, TokenKind.Comment),
            new("const", 1, 0, 5, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 6, 6, TokenKind.Identificator),
            new("2", 1, 13, 1, TokenKind.IntegerNumber)
        });
        Assert.AreEqual(0, _codeModel.Imports.Count);
        Assert.AreEqual(2, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.ConstNoValue, _result.Errors.First().Type);
        Assert.AreEqual(new Token("const", 0, 0, 5, TokenKind.ConstDeclaration), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void ConstDeclaration_UnexpectedFirstConst()
    {
        // 123
        // const HEIGHT 2
        Parse(new Token[]
        {
            new("123", 0, 0, 3, TokenKind.IntegerNumber),
            new("const", 1, 0, 5, TokenKind.ConstDeclaration),
            new("HEIGHT", 1, 6, 6, TokenKind.Identificator),
            new("2", 1, 13, 1, TokenKind.IntegerNumber)
        });
        Assert.AreEqual(0, _codeModel.Imports.Count);
        Assert.AreEqual(1, _codeModel.Constants.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.UnexpectedToken, _result.Errors.First().Type);
        Assert.AreEqual(new Token("123", 0, 0, 3, TokenKind.IntegerNumber), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void FunctionDeclaration_LineAndColumn()
    {
        // \r\n  (func () 1)
        Parse(new Token[]
        {
            new("(", 1, 2, 1, TokenKind.OpenBracket),
            new("func", 1, 3, 4, TokenKind.Identificator),
            new("(", 1, 8, 1, TokenKind.OpenBracket),
            new(")", 1, 8, 1, TokenKind.CloseBracket),
            new("1", 1, 11, 1, TokenKind.IntegerNumber),
            new(")", 1, 12, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual(1, func.LineIndex);
        Assert.AreEqual(3, func.ColumnIndex);
        Assert.AreEqual(1, func.Body.Count);
        var body = (IntegerValueElement)func.Body.First();
        Assert.AreEqual(1, body.LineIndex);
        Assert.AreEqual(11, body.ColumnIndex);
    }

    [Test]
    public void FunctionDeclaration_IncorrectFunctionName()
    {
        // ('func' () 1)
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("'func'", 0, 1, 6, TokenKind.String),
            new("(", 0, 8, 1, TokenKind.OpenBracket),
            new(")", 0, 9, 1, TokenKind.CloseBracket),
            new("1", 0, 11, 1, TokenKind.IntegerNumber),
            new(")", 0, 12, 1, TokenKind.CloseBracket)
        });
        Assert.AreEqual(0, _codeModel.Functions.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectFunctionName, _result.Errors.First().Type);
        Assert.AreEqual(new Token("'func'", 0, 1, 6, TokenKind.String), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void FunctionDeclaration_IncorrectAgrumentsDeclaration()
    {
        // (func 1 1)
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("1", 0, 6, 1, TokenKind.IntegerNumber),
            new("1", 0, 8, 1, TokenKind.IntegerNumber),
            new(")", 0, 9, 1, TokenKind.CloseBracket)
        });
        Assert.AreEqual(0, _codeModel.Functions.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectFunctionAgrumentsDeclaration, _result.Errors.First().Type);
        Assert.AreEqual(new Token("1", 0, 6, 1, TokenKind.IntegerNumber), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void FunctionDeclaration_IncorrectAgrument()
    {
        // \r\n  (func (1) 1)
        Parse(new Token[]
        {
            new("(", 1, 2, 1, TokenKind.OpenBracket),
            new("func", 1, 3, 4, TokenKind.Identificator),
            new("(", 1, 8, 1, TokenKind.OpenBracket),
            new("1", 1, 9, 1, TokenKind.IntegerNumber),
            new(")", 1, 9, 1, TokenKind.CloseBracket),
            new("1", 1, 12, 1, TokenKind.IntegerNumber),
            new(")", 1, 13, 1, TokenKind.CloseBracket)
        });
        Assert.AreEqual(0, _codeModel.Functions.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectFunctionAgrument, _result.Errors.First().Type);
        Assert.AreEqual(new Token("1", 1, 9, 1, TokenKind.IntegerNumber), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void FunctionDeclaration_UnexpectedEnd()
    {
        // (func () 1
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("1", 0, 9, 1, TokenKind.IntegerNumber)
        });
        Assert.AreEqual(1, _codeModel.Functions.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.UnexpectedFunctionEnd, _result.Errors.First().Type);
        Assert.AreEqual(new Token("1", 0, 9, 1, TokenKind.IntegerNumber), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void FunctionDeclaration_Arguments_Correct()
    {
        // (func (xxx yyy zzz) 1)
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new("xxx", 0, 7, 3, TokenKind.Identificator),
            new("yyy", 0, 11, 3, TokenKind.Identificator),
            new("zzz", 0, 15, 3, TokenKind.Identificator),
            new(")", 0, 18, 1, TokenKind.CloseBracket),
            new("1", 0, 20, 1, TokenKind.IntegerNumber),
            new(")", 0, 21, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual(3, func.Arguments.Count);
        Assert.AreEqual("xxx", func.Arguments[0].Name);
        Assert.AreEqual(0, func.Arguments[0].LineIndex);
        Assert.AreEqual(7, func.Arguments[0].ColumnIndex);
        Assert.AreEqual("yyy", func.Arguments[1].Name);
        Assert.AreEqual(0, func.Arguments[1].LineIndex);
        Assert.AreEqual(11, func.Arguments[1].ColumnIndex);
        Assert.AreEqual("zzz", func.Arguments[2].Name);
        Assert.AreEqual(0, func.Arguments[2].LineIndex);
        Assert.AreEqual(15, func.Arguments[2].ColumnIndex);
    }

    [Test]
    public void FunctionDeclaration_Body_IncorrectToken()
    {
        // (func () ^)
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("^", 0, 9, 1, TokenKind.Unknown)
        });
        Assert.AreEqual(1, _codeModel.Functions.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectToken, _result.Errors.First().Type);
        Assert.AreEqual(new Token("^", 0, 9, 1, TokenKind.Unknown), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void FunctionDeclaration_Arguments_LastIncorrect()
    {
        // (func (xxx yyy 111) 1)
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new("xxx", 0, 7, 3, TokenKind.Identificator),
            new("yyy", 0, 11, 3, TokenKind.Identificator),
            new("111", 0, 15, 3, TokenKind.IntegerNumber),
            new(")", 0, 18, 1, TokenKind.CloseBracket),
            new("1", 0, 20, 1, TokenKind.IntegerNumber),
            new(")", 0, 21, 1, TokenKind.CloseBracket)
        });
        Assert.AreEqual(0, _codeModel.Functions.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectFunctionAgrument, _result.Errors.First().Type);
        Assert.AreEqual(new Token("111", 0, 15, 3, TokenKind.IntegerNumber), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void FunctionDeclaration_Arguments_NoCloseBracket()
    {
        // (func (xxx
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new("xxx", 0, 7, 3, TokenKind.Identificator)
        });
        Assert.AreEqual(0, _codeModel.Functions.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectFunctionAgrumentsDeclaration, _result.Errors.First().Type);
        Assert.AreEqual(new Token("", 0, 0, 0, TokenKind.Unknown), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void FunctionDeclaration_OneIntegerConstant()
    {
        // (func () 1)
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("1", 0, 9, 1, TokenKind.IntegerNumber),
            new(")", 0, 10, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual("func", func.Name);
        Assert.AreEqual(0, func.Arguments.Count);
        Assert.AreEqual(1, func.Body.Count);
        Assert.True(func.Body.First() is IntegerValueElement);
        var body = (IntegerValueElement)func.Body.First();
        Assert.AreEqual(1, body.Value);
    }

    [Test]
    public void FunctionDeclaration_IntegerValueOverflow()
    {
        // (func () 11111111111111111111111111111111111111111111111111)
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("11111111111111111111111111111111111111111111111111", 0, 9, 50, TokenKind.IntegerNumber),
            new(")", 0, 51, 1, TokenKind.CloseBracket)
        });
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IntegerValueOverflow, _result.Errors.First().Type);
        Assert.AreEqual(new Token("11111111111111111111111111111111111111111111111111", 0, 9, 50, TokenKind.IntegerNumber), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
    }

    [Test]
    public void FunctionDeclaration_OneFloatConstant()
    {
        // (func () 1.2)
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("1.2", 0, 9, 3, TokenKind.FloatNumber),
            new(")", 0, 12, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual("func", func.Name);
        Assert.AreEqual(0, func.Arguments.Count);
        Assert.AreEqual(1, func.Body.Count);
        Assert.True(func.Body.First() is FloatValueElement);
        var body = (FloatValueElement)func.Body.First();
        Assert.AreEqual(1.2, body.Value);
    }

    [Test]
    public void FunctionDeclaration_OneStringConstant()
    {
        // (func () 'str')
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("'str'", 0, 9, 5, TokenKind.String),
            new(")", 0, 14, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual("func", func.Name);
        Assert.AreEqual(0, func.Arguments.Count);
        Assert.AreEqual(1, func.Body.Count);
        Assert.True(func.Body.First() is StringValueElement);
        var body = (StringValueElement)func.Body.First();
        Assert.AreEqual("str", body.Value);
        Assert.AreEqual(0, body.LineIndex);
        Assert.AreEqual(9, body.ColumnIndex);
    }

    [Test]
    public void FunctionDeclaration_BooleanList()
    {
        // (func () (true false))
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("(", 0, 9, 1, TokenKind.OpenBracket),
            new("true", 0, 10, 4, TokenKind.BooleanTrue),
            new("false", 0, 15, 5, TokenKind.BooleanFalse),
            new(")", 0, 20, 1, TokenKind.CloseBracket),
            new(")", 0, 21, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual("func", func.Name);
        Assert.AreEqual(0, func.Arguments.Count);
        Assert.AreEqual(1, func.Body.Count);
        Assert.True(func.Body.First() is ListValueElement);
        var body = (ListValueElement)func.Body.First();
        Assert.AreEqual(2, body.Items.Count);
        Assert.True(body.Items[0] is BooleanValueElement);
        Assert.True(body.Items[1] is BooleanValueElement);
        Assert.AreEqual(true, ((BooleanValueElement)body.Items[0]).Value);
        Assert.AreEqual(false, ((BooleanValueElement)body.Items[1]).Value);
    }

    [Test]
    public void FunctionDeclaration_OneList()
    {
        // (func () (1 1.2 'str'))
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("(", 0, 9, 1, TokenKind.OpenBracket),
            new("1", 0, 10, 1, TokenKind.IntegerNumber),
            new("1.2", 0, 12, 3, TokenKind.FloatNumber),
            new("'str'", 0, 16, 5, TokenKind.String),
            new(")", 0, 21, 1, TokenKind.CloseBracket),
            new(")", 0, 22, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual("func", func.Name);
        Assert.AreEqual(0, func.Arguments.Count);
        Assert.AreEqual(1, func.Body.Count);
        Assert.True(func.Body.First() is ListValueElement);
        var body = (ListValueElement)func.Body.First();
        Assert.AreEqual(0, body.LineIndex);
        Assert.AreEqual(9, body.ColumnIndex);
        Assert.AreEqual(3, body.Items.Count);
        Assert.AreEqual(typeof(IntegerValueElement), body.Items[0].GetType());
        Assert.AreEqual(typeof(FloatValueElement), body.Items[1].GetType());
        Assert.AreEqual(typeof(StringValueElement), body.Items[2].GetType());
    }

    [Test]
    public void FunctionDeclaration_List_NoCloseBracket()
    {
        // (func () (1 1.2 'str'
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("(", 0, 9, 1, TokenKind.OpenBracket),
            new("1", 0, 10, 1, TokenKind.IntegerNumber),
            new("1.2", 0, 12, 3, TokenKind.FloatNumber),
            new("'str'", 0, 16, 5, TokenKind.String)
        });
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectToken, _result.Errors.First().Type);
        Assert.AreEqual(new Token("", 0, 0, 0, TokenKind.Unknown), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.IsNull(_codeModel.RunFunction);
    }

    [Test]
    public void FunctionDeclaration_OneNamedConstant()
    {
        // (func () WIDTH)
        _scopeMock.Setup(x => x.IsConstantExist("WIDTH")).Returns(true);
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("WIDTH", 0, 9, 5, TokenKind.Identificator),
            new(")", 0, 14, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual("func", func.Name);
        Assert.AreEqual(0, func.Arguments.Count);
        Assert.AreEqual(1, func.Body.Count);
        Assert.True(func.Body.First() is NamedConstantValueElement);
        var body = (NamedConstantValueElement)func.Body.First();
        Assert.AreEqual("WIDTH", body.Name);
    }

    [Test]
    public void FunctionDeclaration_OneNamedConstantInList()
    {
        // (func () (WIDTH))
        _scopeMock.Setup(x => x.IsConstantExist("WIDTH")).Returns(true);
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("(", 0, 9, 1, TokenKind.OpenBracket),
            new("WIDTH", 0, 10, 5, TokenKind.Identificator),
            new(")", 0, 15, 1, TokenKind.CloseBracket),
            new(")", 0, 16, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual("func", func.Name);
        Assert.AreEqual(0, func.Arguments.Count);
        Assert.AreEqual(1, func.Body.Count);
        Assert.True(func.Body.First() is ListValueElement);
        var body = (ListValueElement)func.Body.First();
        Assert.AreEqual(1, body.Items.Count);
        Assert.AreEqual(typeof(NamedConstantValueElement), body.Items.First().GetType());
        Assert.AreEqual("WIDTH", ((NamedConstantValueElement)body.Items.First()).Name);
    }

    [Test]
    public void FunctionDeclaration_Function()
    {
        // (func () print)
        _scopeMock.Setup(x => x.IsFunctionExist("print")).Returns(true);
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("print", 0, 9, 5, TokenKind.Identificator),
            new(")", 0, 14, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual("func", func.Name);
        Assert.AreEqual(0, func.Arguments.Count);
        Assert.AreEqual(1, func.Body.Count);
        Assert.True(func.Body.First() is FunctionValueElement);
        var body = (FunctionValueElement)func.Body.First();
        Assert.AreEqual("print", body.Name);
        Assert.AreEqual(0, body.LineIndex);
        Assert.AreEqual(9, body.ColumnIndex);
        Assert.AreEqual(0, body.ArgumentValues.Count);
    }

    [Test]
    public void FunctionDeclaration_FunctionAlreadyExist()
    {
        // (func () print)
        _scopeMock.Setup(x => x.IsFunctionExist("func")).Returns(true);
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("print", 0, 9, 5, TokenKind.Identificator),
            new(")", 0, 14, 1, TokenKind.CloseBracket)
        });
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.FunctionNameExist, _result.Errors.First().Type);
        Assert.AreEqual(new Token("func", 0, 1, 4, TokenKind.Identificator), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(0, _codeModel.Functions.Count);
    }

    [Test]
    public void FunctionDeclaration_ConstAlreadyExist()
    {
        // (func () print)
        _scopeMock.Setup(x => x.IsConstantExist("func")).Returns(true);
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new(")", 0, 7, 1, TokenKind.CloseBracket),
            new("print", 0, 9, 5, TokenKind.Identificator),
            new(")", 0, 14, 1, TokenKind.CloseBracket)
        });
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.ConstNameExist, _result.Errors.First().Type);
        Assert.AreEqual(new Token("func", 0, 1, 4, TokenKind.Identificator), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(0, _codeModel.Functions.Count);
    }

    [Test]
    public void FunctionDeclaration_Variable()
    {
        // (func (x) x)
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("func", 0, 1, 4, TokenKind.Identificator),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new("x", 0, 7, 1, TokenKind.Identificator),
            new(")", 0, 8, 1, TokenKind.CloseBracket),
            new("x", 0, 10, 1, TokenKind.Identificator),
            new(")", 0, 11, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual("func", func.Name);
        Assert.AreEqual(1, func.Arguments.Count);
        Assert.AreEqual(1, func.Body.Count);
        Assert.True(func.Body.First() is FunctionArgumentValueElement);
        var body = (FunctionArgumentValueElement)func.Body.First();
        Assert.AreEqual("x", body.Name);
    }

    [Test]
    public void FunctionDeclaration_OneFunctionCall()
    {
        // (f () (g 1 1.2 'str'))
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("f", 0, 1, 1, TokenKind.Identificator),
            new("(", 0, 3, 1, TokenKind.OpenBracket),
            new(")", 0, 4, 1, TokenKind.CloseBracket),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new("g", 0, 7, 1, TokenKind.Identificator),
            new("1", 0, 9, 1, TokenKind.IntegerNumber),
            new("1.2", 0, 11, 3, TokenKind.FloatNumber),
            new("'str'", 0, 15, 5, TokenKind.String),
            new(")", 0, 20, 1, TokenKind.CloseBracket),
            new(")", 0, 21, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual(1, func.Body.Count);
        Assert.True(func.Body.First() is FunctionValueElement);
        var body = (FunctionValueElement)func.Body.First();
        Assert.AreEqual("g", body.Name);
        Assert.AreEqual(3, body.ArgumentValues.Count);
        Assert.AreEqual(1, ((IntegerValueElement)body.ArgumentValues[0]).Value);
        Assert.AreEqual(1.2, ((FloatValueElement)body.ArgumentValues[1]).Value);
        Assert.AreEqual("str", ((StringValueElement)body.ArgumentValues[2]).Value);
    }

    [Test]
    public void FunctionDeclaration_InnerFunctionCall()
    {
        // (f () (g 1 (k 1 2) 'str'))
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("f", 0, 1, 1, TokenKind.Identificator),
            new("(", 0, 3, 1, TokenKind.OpenBracket),
            new(")", 0, 4, 1, TokenKind.CloseBracket),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new("g", 0, 7, 1, TokenKind.Identificator),
            new("1", 0, 9, 1, TokenKind.IntegerNumber),
            new("(", 0, 11, 1, TokenKind.OpenBracket),
            new("k", 0, 12, 1, TokenKind.Identificator),
            new("1", 0, 14, 1, TokenKind.IntegerNumber),
            new("2", 0, 16, 1, TokenKind.IntegerNumber),
            new(")", 0, 17, 1, TokenKind.CloseBracket),
            new("'str'", 0, 19, 5, TokenKind.String),
            new(")", 0, 24, 1, TokenKind.CloseBracket),
            new(")", 0, 25, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual(1, func.Body.Count);
        Assert.True(func.Body.First() is FunctionValueElement);
        var body = (FunctionValueElement)func.Body.First();
        var innerbody = (FunctionValueElement)body.ArgumentValues[1];
        Assert.AreEqual("k", innerbody.Name);
        Assert.AreEqual(0, innerbody.LineIndex);
        Assert.AreEqual(12, innerbody.ColumnIndex);
        Assert.AreEqual(2, innerbody.ArgumentValues.Count);
        Assert.AreEqual(1, ((IntegerValueElement)innerbody.ArgumentValues[0]).Value);
        Assert.AreEqual(2, ((IntegerValueElement)innerbody.ArgumentValues[1]).Value);
    }

    [Test]
    public void FunctionDeclaration_FunctionCall_IncorrectBody()
    {
        // (f () (g ^
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("f", 0, 1, 1, TokenKind.Identificator),
            new("(", 0, 3, 1, TokenKind.OpenBracket),
            new(")", 0, 4, 1, TokenKind.CloseBracket),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new("g", 0, 7, 1, TokenKind.Identificator),
            new("^", 0, 9, 1, TokenKind.Unknown)
        });
        Assert.AreEqual(1, _codeModel.Functions.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectToken, _result.Errors.First().Type);
        Assert.AreEqual(new Token("^", 0, 9, 1, TokenKind.Unknown), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void FunctionDeclaration_FunctionCall_NoCloseBracket()
    {
        // (f () (g
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("f", 0, 1, 1, TokenKind.Identificator),
            new("(", 0, 3, 1, TokenKind.OpenBracket),
            new(")", 0, 4, 1, TokenKind.CloseBracket),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new("g", 0, 7, 1, TokenKind.Identificator)
        });
        Assert.AreEqual(1, _codeModel.Functions.Count);
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.UnexpectedToken, _result.Errors.First().Type);
        Assert.AreEqual(new Token("", 0, 0, 0, TokenKind.Unknown), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
    }

    [Test]
    public void FunctionDeclaration_Lambda()
    {
        // (f () (lambda (x) (g x)))
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("f", 0, 1, 1, TokenKind.Identificator),
            new("(", 0, 3, 1, TokenKind.OpenBracket),
            new(")", 0, 4, 1, TokenKind.CloseBracket),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new("lambda", 0, 7, 6, TokenKind.Lambda),
            new("(", 0, 14, 1, TokenKind.OpenBracket),
            new("x", 0, 15, 1, TokenKind.Identificator),
            new(")", 0, 16, 1, TokenKind.CloseBracket),
            new("(", 0, 18, 1, TokenKind.OpenBracket),
            new("g", 0, 19, 1, TokenKind.Identificator),
            new("x", 0, 21, 1, TokenKind.Identificator),
            new(")", 0, 22, 1, TokenKind.CloseBracket),
            new(")", 0, 23, 1, TokenKind.CloseBracket),
            new(")", 0, 24, 1, TokenKind.CloseBracket)
        });

        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual(1, func.Body.Count);
        Assert.True(func.Body.First() is LambdaValueElement);
        var body = (LambdaValueElement)func.Body.First();
        Assert.AreEqual(1, body.Arguments.Count);

        Assert.AreEqual(1, body.Body.Count);
        Assert.True(body.Body.First() is FunctionValueElement);
        var lambdaBody = (FunctionValueElement)body.Body.First();
        Assert.AreEqual("g", lambdaBody.Name);
        Assert.AreEqual(1, lambdaBody.ArgumentValues.Count);
        Assert.AreEqual("x", ((FunctionArgumentValueElement)lambdaBody.ArgumentValues[0]).Name);
    }

    [Test]
    public void FunctionDeclaration_Lambda_TwoItemsBody()
    {
        // (f () (lambda (x) (g x) (h x)))
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("f", 0, 1, 1, TokenKind.Identificator),
            new("(", 0, 3, 1, TokenKind.OpenBracket),
            new(")", 0, 4, 1, TokenKind.CloseBracket),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new("lambda", 0, 7, 6, TokenKind.Lambda),
            new("(", 0, 14, 1, TokenKind.OpenBracket),
            new("x", 0, 15, 1, TokenKind.Identificator),
            new(")", 0, 16, 1, TokenKind.CloseBracket),
            new("(", 0, 18, 1, TokenKind.OpenBracket),
            new("g", 0, 19, 1, TokenKind.Identificator),
            new("x", 0, 21, 1, TokenKind.Identificator),
            new(")", 0, 22, 1, TokenKind.CloseBracket),
            new("(", 0, 24, 1, TokenKind.OpenBracket),
            new("h", 0, 25, 1, TokenKind.Identificator),
            new("x", 0, 27, 1, TokenKind.Identificator),
            new(")", 0, 28, 1, TokenKind.CloseBracket),
            new(")", 0, 29, 1, TokenKind.CloseBracket),
            new(")", 0, 30, 1, TokenKind.CloseBracket)
        });

        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual(1, func.Body.Count);
        Assert.True(func.Body.First() is LambdaValueElement);
        var body = (LambdaValueElement)func.Body.First();
        Assert.AreEqual(1, body.Arguments.Count);

        Assert.AreEqual(2, body.Body.Count);
        Assert.True(body.Body.First() is FunctionValueElement);
        var lambdaBody = (FunctionValueElement)body.Body.First();
        Assert.AreEqual("g", lambdaBody.Name);
        Assert.AreEqual(1, lambdaBody.ArgumentValues.Count);
        Assert.AreEqual("x", ((FunctionArgumentValueElement)lambdaBody.ArgumentValues[0]).Name);

        Assert.True(body.Body.Last() is FunctionValueElement);
        lambdaBody = (FunctionValueElement)body.Body.Last();
        Assert.AreEqual("h", lambdaBody.Name);
        Assert.AreEqual(1, lambdaBody.ArgumentValues.Count);
        Assert.AreEqual("x", ((FunctionArgumentValueElement)lambdaBody.ArgumentValues[0]).Name);
    }

    [Test]
    public void FunctionDeclaration_Lambda_IncorrectArguments()
    {
        // (f () (lambda (^
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("f", 0, 1, 1, TokenKind.Identificator),
            new("(", 0, 3, 1, TokenKind.OpenBracket),
            new(")", 0, 4, 1, TokenKind.CloseBracket),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new("lambda", 0, 7, 6, TokenKind.Lambda),
            new("(", 0, 14, 1, TokenKind.OpenBracket),
            new("^", 0, 15, 1, TokenKind.Unknown)
        });
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectFunctionAgrument, _result.Errors.First().Type);
        Assert.AreEqual(new Token("^", 0, 15, 1, TokenKind.Unknown), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
    }

    [Test]
    public void FunctionDeclaration_Lambda_IncorrectBody()
    {
        // (f () (lambda (x) (^
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("f", 0, 1, 1, TokenKind.Identificator),
            new("(", 0, 3, 1, TokenKind.OpenBracket),
            new(")", 0, 4, 1, TokenKind.CloseBracket),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new("lambda", 0, 7, 6, TokenKind.Lambda),
            new("(", 0, 14, 1, TokenKind.OpenBracket),
            new("x", 0, 15, 1, TokenKind.Identificator),
            new(")", 0, 16, 1, TokenKind.CloseBracket),
            new("(", 0, 18, 1, TokenKind.OpenBracket),
            new("^", 0, 19, 1, TokenKind.Unknown)
        });
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectToken, _result.Errors.First().Type);
        Assert.AreEqual(new Token("^", 0, 19, 1, TokenKind.Unknown), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
    }

    [Test]
    public void FunctionDeclaration_TwoFunctionsInBody()
    {
        // ( f () (g 1) (h 2) )
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("f", 0, 2, 1, TokenKind.Identificator),
            new("(", 0, 4, 1, TokenKind.OpenBracket),
            new(")", 0, 5, 1, TokenKind.CloseBracket),
            new("(", 0, 7, 1, TokenKind.OpenBracket),
            new("g", 0, 8, 1, TokenKind.Identificator),
            new("1", 0, 10, 1, TokenKind.IntegerNumber),
            new(")", 0, 11, 1, TokenKind.CloseBracket),
            new("(", 0, 13, 1, TokenKind.OpenBracket),
            new("h", 0, 14, 1, TokenKind.Identificator),
            new("2", 0, 16, 1, TokenKind.IntegerNumber),
            new(")", 0, 17, 1, TokenKind.CloseBracket),
            new(")", 0, 19, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual(2, func.Body.Count);
    }

    [Test]
    public void FunctionDeclaration_MathOperators()
    {
        // ( f () (+ 1 2) (- 1 2) (* 1 2) (/ 1 2) (% 1 2) )
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),

            new("f", 0, 2, 1, TokenKind.Identificator),
            new("(", 0, 4, 1, TokenKind.OpenBracket),
            new(")", 0, 5, 1, TokenKind.CloseBracket),

            new("(", 0, 7, 1, TokenKind.OpenBracket),
            new("+", 0, 8, 1, TokenKind.Plus),
            new("1", 0, 10, 1, TokenKind.IntegerNumber),
            new("2", 0, 12, 1, TokenKind.IntegerNumber),
            new(")", 0, 13, 1, TokenKind.CloseBracket),

            new("(", 0, 15, 1, TokenKind.OpenBracket),
            new("-", 0, 16, 1, TokenKind.Minus),
            new("1", 0, 18, 1, TokenKind.IntegerNumber),
            new("2", 0, 20, 1, TokenKind.IntegerNumber),
            new(")", 0, 21, 1, TokenKind.CloseBracket),

            new("(", 0, 23, 1, TokenKind.OpenBracket),
            new("*", 0, 24, 1, TokenKind.Asterisk),
            new("1", 0, 26, 1, TokenKind.IntegerNumber),
            new("2", 0, 28, 1, TokenKind.IntegerNumber),
            new(")", 0, 29, 1, TokenKind.CloseBracket),

            new("(", 0, 31, 1, TokenKind.OpenBracket),
            new("/", 0, 32, 1, TokenKind.Slash),
            new("1", 0, 34, 1, TokenKind.IntegerNumber),
            new("2", 0, 36, 1, TokenKind.IntegerNumber),
            new(")", 0, 37, 1, TokenKind.CloseBracket),

            new("(", 0, 39, 1, TokenKind.OpenBracket),
            new("%", 0, 40, 1, TokenKind.Percent),
            new("1", 0, 42, 1, TokenKind.IntegerNumber),
            new("2", 0, 44, 1, TokenKind.IntegerNumber),
            new(")", 0, 45, 1, TokenKind.CloseBracket),

            new(")", 0, 39, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.AreEqual(5, func.Body.Count);
    }

    [Test]
    public void FunctionDeclaration_GetVariable()
    {
        // (f () @var)
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("f", 0, 1, 1, TokenKind.Identificator),
            new("(", 0, 3, 1, TokenKind.OpenBracket),
            new(")", 0, 4, 1, TokenKind.CloseBracket),
            new("@var", 0, 6, 4, TokenKind.Variable),
            new(")", 0, 11, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.True(func.Body.First() is VariableValueElement);
        var body = (VariableValueElement)func.Body.First();
        Assert.AreEqual("@var", body.Name);
    }

    [Test]
    public void FunctionDeclaration_SetVariable()
    {
        // (f () (set @var 1))
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("f", 0, 1, 1, TokenKind.Identificator),
            new("(", 0, 3, 1, TokenKind.OpenBracket),
            new(")", 0, 4, 1, TokenKind.CloseBracket),
            new("(", 0, 6, 1, TokenKind.OpenBracket),
            new("set", 0, 7, 3, TokenKind.Identificator),
            new("@var", 0, 11, 4, TokenKind.Variable),
            new("1", 0, 16, 1, TokenKind.IntegerNumber),
            new(")", 0, 17, 1, TokenKind.CloseBracket),
            new(")", 0, 18, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
        var func = _codeModel.Functions.First();
        Assert.True(func.Body.First() is FunctionValueElement);
        var body = (FunctionValueElement)func.Body.First();
        Assert.AreEqual(2, body.ArgumentValues.Count);
        var variable = body.ArgumentValues[0];
        var value = body.ArgumentValues[1];
        Assert.True(variable is VariableValueElement);
        Assert.True(value is IntegerValueElement);
        Assert.AreEqual("@var", ((VariableValueElement)variable).Name);
    }

    [Test]
    public void FunctionDeclaration_UnexpectedFirstConst()
    {
        // 123
        // (func (x) x)
        Parse(new Token[]
        {
            new("123", 0, 0, 3, TokenKind.IntegerNumber),
            new("(", 1, 0, 1, TokenKind.OpenBracket),
            new("func", 1, 1, 4, TokenKind.Identificator),
            new("(", 1, 6, 1, TokenKind.OpenBracket),
            new("x", 1, 7, 1, TokenKind.Identificator),
            new(")", 1, 8, 1, TokenKind.CloseBracket),
            new("x", 1, 10, 1, TokenKind.Identificator),
            new(")", 1, 11, 1, TokenKind.CloseBracket)
        });
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.UnexpectedToken, _result.Errors.First().Type);
        Assert.AreEqual(new Token("123", 0, 0, 3, TokenKind.IntegerNumber), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.AreEqual(1, _codeModel.Functions.Count);
    }

    [Test]
    public void RunFunctionCall()
    {
        // (run (f 1))
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("run", 0, 1, 3, TokenKind.RunFunction),
            new("(", 0, 5, 1, TokenKind.OpenBracket),
            new("f", 0, 6, 1, TokenKind.Identificator),
            new("1", 0, 8, 1, TokenKind.IntegerNumber),
            new(")", 0, 9, 1, TokenKind.CloseBracket),
            new(")", 0, 10, 1, TokenKind.CloseBracket)
        });
        Assert.False(_result.Errors.Any());
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.NotNull(_codeModel.RunFunction);
        var run = _codeModel.RunFunction!;
        Assert.AreEqual("f", run.Name);
        Assert.AreEqual(1, run.ArgumentValues.Count);
        Assert.AreEqual(typeof(IntegerValueElement), run.ArgumentValues[0].GetType());
    }

    [Test]
    public void RunFunctionExist()
    {
        // (run (f 1))
        _scopeMock.Setup(x => x.IsFunctionExist("f")).Returns(true);
        _scopeMock.Setup(x => x.IsRunFunctionExist()).Returns(true);
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("run", 0, 1, 3, TokenKind.RunFunction),
            new("(", 0, 5, 1, TokenKind.OpenBracket),
            new("f", 0, 6, 1, TokenKind.Identificator),
            new("1", 0, 8, 1, TokenKind.IntegerNumber),
            new(")", 0, 9, 1, TokenKind.CloseBracket),
            new(")", 0, 10, 1, TokenKind.CloseBracket)
        });
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.RunFunctionExist, _result.Errors.First().Type);
        Assert.AreEqual(new Token("run", 0, 1, 3, TokenKind.RunFunction), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.IsNull(_codeModel.RunFunction);
    }

    [Test]
    public void RunFunctionCall_IncorrectFunctionCall()
    {
        // (run (f
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("run", 0, 1, 3, TokenKind.RunFunction),
            new("(", 0, 5, 1, TokenKind.OpenBracket),
            new("f", 0, 6, 1, TokenKind.Identificator)
        });
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.IncorrectFunctionCall, _result.Errors.First().Type);
        Assert.AreEqual(new Token("", 0, 0, 0, TokenKind.Unknown), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.IsNull(_codeModel.RunFunction);
    }

    [Test]
    public void RunFunctionCall_NoCloseBracket()
    {
        // (run (f 1)
        Parse(new Token[]
        {
            new("(", 0, 0, 1, TokenKind.OpenBracket),
            new("run", 0, 1, 3, TokenKind.RunFunction),
            new("(", 0, 5, 1, TokenKind.OpenBracket),
            new("f", 0, 6, 1, TokenKind.Identificator),
            new("1", 0, 8, 1, TokenKind.IntegerNumber),
            new(")", 0, 9, 1, TokenKind.CloseBracket)
        });
        Assert.AreEqual(1, _result.Errors.Count);
        Assert.AreEqual(ParserMessageType.UnexpectedToken, _result.Errors.First().Type);
        Assert.AreEqual(new Token("", 0, 0, 0, TokenKind.Unknown), _result.Errors.First().Token);
        Assert.AreEqual(0, _result.Warnings.Count);
        Assert.IsNull(_codeModel.RunFunction);
    }

    private void Parse(IEnumerable<Token> tokens)
    {
        _parser = new FunctionParser(new TokenIterator(tokens), _codeModel, _scopeMock.Object);
        _result = _parser.Parse();
    }
}
