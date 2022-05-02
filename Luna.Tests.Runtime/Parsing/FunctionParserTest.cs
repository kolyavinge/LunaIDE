using System.Collections.Generic;
using System.Linq;
using Luna.Parsing;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Parsing
{
    public class FunctionParserTest
    {
        private Mock<IScope> _scopeMock;
        private CodeModel _codeModel;
        private FunctionParser _parser;
        private ParseResult _result;

        [SetUp]
        public void Setup()
        {
            _scopeMock = new Mock<IScope>();
            _scopeMock.Setup(x => x.IsConstExist(It.IsAny<string>())).Returns(false);
            _scopeMock.Setup(x => x.IsFunctionExist(It.IsAny<string>())).Returns(false);
            _codeModel = new CodeModel();
        }

        [Test]
        public void ConstDeclaration_Correct_Integer()
        {
            Parse("const WIDTH 123 // comment", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
                new Token(0, 6, 5, TokenKind.Identificator),
                new Token(0, 12, 3, TokenKind.IntegerNumber),
                new Token(0, 16, 10, TokenKind.Comment),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Constants.Count);
            Assert.AreEqual("WIDTH", _codeModel.Constants.First().Name);
            Assert.AreEqual(123, ((IntegerValue)_codeModel.Constants.First().Value).Value);
            Assert.AreEqual(0, _codeModel.Constants.First().LineIndex);
            Assert.AreEqual(0, _codeModel.Constants.First().ColumnIndex);
        }

        [Test]
        public void ConstDeclaration_Correct_Float()
        {
            Parse("const WIDTH 1.23 // comment", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
                new Token(0, 6, 5, TokenKind.Identificator),
                new Token(0, 12, 4, TokenKind.FloatNumber),
                new Token(0, 17, 10, TokenKind.Comment),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Constants.Count);
            Assert.AreEqual("WIDTH", _codeModel.Constants.First().Name);
            Assert.AreEqual(typeof(FloatValue), _codeModel.Constants.First().Value.GetType());
            Assert.AreEqual(1.23, ((FloatValue)_codeModel.Constants.First().Value).Value);
            Assert.AreEqual(0, _codeModel.Constants.First().LineIndex);
            Assert.AreEqual(0, _codeModel.Constants.First().ColumnIndex);
        }

        [Test]
        public void ConstDeclaration_Correct_String()
        {
            Parse("const WIDTH '123' // comment", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
                new Token(0, 6, 5, TokenKind.Identificator),
                new Token(0, 12, 5, TokenKind.String),
                new Token(0, 18, 10, TokenKind.Comment),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Constants.Count);
            Assert.AreEqual("WIDTH", _codeModel.Constants.First().Name);
            Assert.AreEqual("123", ((StringValue)_codeModel.Constants.First().Value).Value);
            Assert.AreEqual(0, _codeModel.Constants.First().LineIndex);
            Assert.AreEqual(0, _codeModel.Constants.First().ColumnIndex);
        }

        [Test]
        public void ConstDeclaration_Correct_2()
        {
            Parse("const WIDTH 123\r\nconst VALUE 456", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
                new Token(0, 6, 5, TokenKind.Identificator),
                new Token(0, 12, 3, TokenKind.IntegerNumber),
                new Token(1, 0, 5, TokenKind.ConstDeclaration),
                new Token(1, 6, 5, TokenKind.Identificator),
                new Token(1, 12, 3, TokenKind.IntegerNumber),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(2, _codeModel.Constants.Count);
        }

        [Test]
        public void ConstDeclaration_ConstNameExist()
        {
            _scopeMock.Setup(x => x.IsConstExist("WIDTH")).Returns(true);
            Parse("const WIDTH 123 // comment", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
                new Token(0, 6, 5, TokenKind.Identificator),
                new Token(0, 12, 3, TokenKind.IntegerNumber),
                new Token(0, 16, 10, TokenKind.Comment),
            });
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.ConstNameExist, _result.Error.Type);
            Assert.AreEqual(new Token(0, 6, 5, TokenKind.Identificator), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ConstDeclaration_FunctionNameExist()
        {
            _scopeMock.Setup(x => x.IsFunctionExist("WIDTH")).Returns(true);
            Parse("const WIDTH 123 // comment", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
                new Token(0, 6, 5, TokenKind.Identificator),
                new Token(0, 12, 3, TokenKind.IntegerNumber),
                new Token(0, 16, 10, TokenKind.Comment),
            });
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.FunctionNameExist, _result.Error.Type);
            Assert.AreEqual(new Token(0, 6, 5, TokenKind.Identificator), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ConstDeclaration_EmptyDeclaration()
        {
            Parse("const ", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
            });
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.ConstEmptyDeclaration, _result.Error.Type);
            Assert.AreEqual(new Token(0, 0, 5, TokenKind.ConstDeclaration), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ConstDeclaration_IncorrectName_String()
        {
            Parse("const '123'", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
                new Token(0, 6, 5, TokenKind.String),
            });
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.ConstIncorrectName, _result.Error.Type);
            Assert.AreEqual(new Token(0, 6, 5, TokenKind.String), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ConstDeclaration_IncorrectName_Integer()
        {
            Parse("const 123", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
                new Token(0, 6, 3, TokenKind.IntegerNumber),
            });
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.ConstIncorrectName, _result.Error.Type);
            Assert.AreEqual(new Token(0, 6, 3, TokenKind.IntegerNumber), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ConstDeclaration_IncorrectName_Float()
        {
            Parse("const 1.23", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
                new Token(0, 6, 4, TokenKind.FloatNumber),
            });
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.ConstIncorrectName, _result.Error.Type);
            Assert.AreEqual(new Token(0, 6, 4, TokenKind.FloatNumber), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ConstDeclaration_NoValue()
        {
            Parse("const WIDTH", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
                new Token(0, 6, 5, TokenKind.Identificator),
            });
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.ConstNoValue, _result.Error.Type);
            Assert.AreEqual(new Token(0, 0, 5, TokenKind.ConstDeclaration), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ConstDeclaration_IncorrectValue()
        {
            Parse("const WIDTH VALUE", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
                new Token(0, 6, 5, TokenKind.Identificator),
                new Token(0, 12, 5, TokenKind.Identificator),
            });
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.ConstIncorrectValue, _result.Error.Type);
            Assert.AreEqual(new Token(0, 12, 5, TokenKind.Identificator), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ConstDeclaration_UnexpectedTokens()
        {
            Parse("const WIDTH 123 const VALUE 456", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
                new Token(0, 6, 5, TokenKind.Identificator),
                new Token(0, 12, 3, TokenKind.IntegerNumber),
                new Token(0, 16, 5, TokenKind.ConstDeclaration),
                new Token(0, 22, 5, TokenKind.Identificator),
                new Token(0, 28, 3, TokenKind.IntegerNumber),
            });
            Assert.AreEqual(0, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.UnexpectedToken, _result.Error.Type);
            Assert.AreEqual(new Token(0, 16, 5, TokenKind.ConstDeclaration), _result.Error.Tokens[0]);
            Assert.AreEqual(new Token(0, 22, 5, TokenKind.Identificator), _result.Error.Tokens[1]);
            Assert.AreEqual(new Token(0, 28, 3, TokenKind.IntegerNumber), _result.Error.Tokens[2]);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void ConstDeclaration_UnexpectedImportDirective()
        {
            Parse("const WIDTH '123'\r\nimport 'file'", new[]
            {
                new Token(0, 0, 5, TokenKind.ConstDeclaration),
                new Token(0, 6, 5, TokenKind.Identificator),
                new Token(0, 12, 5, TokenKind.String),
                new Token(1, 0, 6, TokenKind.ImportDirective),
                new Token(1, 7, 6, TokenKind.String),
            });
            Assert.AreEqual(0, _codeModel.Imports.Count);
            Assert.AreEqual(1, _codeModel.Constants.Count);
            Assert.AreEqual(ParserMessageType.UnexpectedImport, _result.Error.Type);
            Assert.AreEqual(new Token(1, 0, 6, TokenKind.ImportDirective), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void FunctionDeclaration_LineAndColumn()
        {
            Parse("\r\n  (func () 1)", new[]
            {
                new Token(1, 2, 1, TokenKind.OpenBracket),
                new Token(1, 3, 4, TokenKind.Identificator),
                new Token(1, 8, 1, TokenKind.OpenBracket),
                new Token(1, 8, 1, TokenKind.CloseBracket),
                new Token(1, 11, 1, TokenKind.IntegerNumber),
                new Token(1, 12, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Functions.Count);
            var func = _codeModel.Functions.First();
            Assert.AreEqual(1, func.LineIndex);
            Assert.AreEqual(3, func.ColumnIndex);
            Assert.AreEqual(1, func.Body.Count);
            var body = (IntegerValue)func.Body.First();
            Assert.AreEqual(1, body.LineIndex);
            Assert.AreEqual(11, body.ColumnIndex);
        }

        [Test]
        public void FunctionDeclaration_IncorrectFunctionName()
        {
            Parse("('func' () 1)", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 6, TokenKind.String),
                new Token(0, 8, 1, TokenKind.OpenBracket),
                new Token(0, 9, 1, TokenKind.CloseBracket),
                new Token(0, 11, 1, TokenKind.IntegerNumber),
                new Token(0, 12, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(0, _codeModel.Functions.Count);
            Assert.AreEqual(ParserMessageType.IncorrectFunctionName, _result.Error.Type);
            Assert.AreEqual(new Token(0, 1, 6, TokenKind.String), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void FunctionDeclaration_IncorrectAgrumentsDeclaration()
        {
            Parse("(func 1 1)", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 4, TokenKind.Identificator),
                new Token(0, 6, 1, TokenKind.IntegerNumber),
                new Token(0, 8, 1, TokenKind.IntegerNumber),
                new Token(0, 9, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(0, _codeModel.Functions.Count);
            Assert.AreEqual(ParserMessageType.IncorrectFunctionAgrumentsDeclaration, _result.Error.Type);
            Assert.AreEqual(new Token(0, 6, 1, TokenKind.IntegerNumber), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void FunctionDeclaration_IncorrectAgrument()
        {
            Parse("\r\n  (func (1) 1)", new[]
            {
                new Token(1, 2, 1, TokenKind.OpenBracket),
                new Token(1, 3, 4, TokenKind.Identificator),
                new Token(1, 8, 1, TokenKind.OpenBracket),
                new Token(1, 9, 1, TokenKind.IntegerNumber),
                new Token(1, 9, 1, TokenKind.CloseBracket),
                new Token(1, 12, 1, TokenKind.IntegerNumber),
                new Token(1, 13, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(0, _codeModel.Functions.Count);
            Assert.AreEqual(ParserMessageType.IncorrectFunctionAgrument, _result.Error.Type);
            Assert.AreEqual(new Token(1, 9, 1, TokenKind.IntegerNumber), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void FunctionDeclaration_UnexpectedEnd()
        {
            Parse("(func () 1", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 4, TokenKind.Identificator),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 1, TokenKind.CloseBracket),
                new Token(0, 9, 1, TokenKind.IntegerNumber),
            });
            Assert.AreEqual(0, _codeModel.Functions.Count);
            Assert.AreEqual(ParserMessageType.UnexpectedFunctionEnd, _result.Error.Type);
            Assert.AreEqual(new Token(0, 9, 1, TokenKind.IntegerNumber), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void FunctionDeclaration_Arguments_Correct()
        {
            Parse("(func (xxx yyy zzz) 1)", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 4, TokenKind.Identificator),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 3, TokenKind.Identificator),
                new Token(0, 11, 3, TokenKind.Identificator),
                new Token(0, 15, 3, TokenKind.Identificator),
                new Token(0, 18, 1, TokenKind.CloseBracket),
                new Token(0, 20, 1, TokenKind.IntegerNumber),
                new Token(0, 21, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(null, _result.Error);
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
        public void FunctionDeclaration_Arguments_LastIncorrect()
        {
            Parse("(func (xxx yyy 111) 1)", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 4, TokenKind.Identificator),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 3, TokenKind.Identificator),
                new Token(0, 11, 3, TokenKind.Identificator),
                new Token(0, 15, 3, TokenKind.IntegerNumber),
                new Token(0, 18, 1, TokenKind.CloseBracket),
                new Token(0, 20, 1, TokenKind.IntegerNumber),
                new Token(0, 21, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(0, _codeModel.Functions.Count);
            Assert.AreEqual(ParserMessageType.IncorrectFunctionAgrument, _result.Error.Type);
            Assert.AreEqual(new Token(0, 15, 3, TokenKind.IntegerNumber), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
        }

        [Test]
        public void FunctionDeclaration_OneIntegerConstant()
        {
            Parse("(func () 1)", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 4, TokenKind.Identificator),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 1, TokenKind.CloseBracket),
                new Token(0, 9, 1, TokenKind.IntegerNumber),
                new Token(0, 10, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Functions.Count);
            var func = _codeModel.Functions.First();
            Assert.AreEqual("func", func.Name);
            Assert.AreEqual(0, func.Arguments.Count);
            Assert.AreEqual(1, func.Body.Count);
            Assert.True(func.Body.First() is IntegerValue);
            var body = (IntegerValue)func.Body.First();
            Assert.AreEqual(1, body.Value);
        }

        [Test]
        public void FunctionDeclaration_OneFloatConstant()
        {
            Parse("(func () 1.2)", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 4, TokenKind.Identificator),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 1, TokenKind.CloseBracket),
                new Token(0, 9, 3, TokenKind.FloatNumber),
                new Token(0, 12, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Functions.Count);
            var func = _codeModel.Functions.First();
            Assert.AreEqual("func", func.Name);
            Assert.AreEqual(0, func.Arguments.Count);
            Assert.AreEqual(1, func.Body.Count);
            Assert.True(func.Body.First() is FloatValue);
            var body = (FloatValue)func.Body.First();
            Assert.AreEqual(1.2, body.Value);
        }

        [Test]
        public void FunctionDeclaration_OneStringConstant()
        {
            Parse("(func () 'str')", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 4, TokenKind.Identificator),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 1, TokenKind.CloseBracket),
                new Token(0, 9, 5, TokenKind.String),
                new Token(0, 14, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Functions.Count);
            var func = _codeModel.Functions.First();
            Assert.AreEqual("func", func.Name);
            Assert.AreEqual(0, func.Arguments.Count);
            Assert.AreEqual(1, func.Body.Count);
            Assert.True(func.Body.First() is StringValue);
            var body = (StringValue)func.Body.First();
            Assert.AreEqual("str", body.Value);
        }

        [Test]
        public void FunctionDeclaration_OneList()
        {
            Parse("(func () (1 1.2 'str'))", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 4, TokenKind.Identificator),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 1, TokenKind.CloseBracket),
                new Token(0, 9, 1, TokenKind.OpenBracket),
                new Token(0, 10, 1, TokenKind.IntegerNumber),
                new Token(0, 12, 3, TokenKind.FloatNumber),
                new Token(0, 16, 5, TokenKind.String),
                new Token(0, 21, 1, TokenKind.CloseBracket),
                new Token(0, 22, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Functions.Count);
            var func = _codeModel.Functions.First();
            Assert.AreEqual("func", func.Name);
            Assert.AreEqual(0, func.Arguments.Count);
            Assert.AreEqual(1, func.Body.Count);
            Assert.True(func.Body.First() is ListValue);
            var body = (ListValue)func.Body.First();
            Assert.AreEqual(3, body.Value.Count);
            Assert.AreEqual(typeof(IntegerValue), body.Value[0].GetType());
            Assert.AreEqual(typeof(FloatValue), body.Value[1].GetType());
            Assert.AreEqual(typeof(StringValue), body.Value[2].GetType());
        }

        [Test]
        public void FunctionDeclaration_OneNamedConstant()
        {
            _scopeMock.Setup(x => x.IsConstExist("WIDTH")).Returns(true);
            Parse("(func () WIDTH)", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 4, TokenKind.Identificator),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 1, TokenKind.CloseBracket),
                new Token(0, 9, 5, TokenKind.Identificator),
                new Token(0, 14, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Functions.Count);
            var func = _codeModel.Functions.First();
            Assert.AreEqual("func", func.Name);
            Assert.AreEqual(0, func.Arguments.Count);
            Assert.AreEqual(1, func.Body.Count);
            Assert.True(func.Body.First() is NamedConstant);
            var body = (NamedConstant)func.Body.First();
            Assert.AreEqual("WIDTH", body.Name);
        }

        [Test]
        public void FunctionDeclaration_Function()
        {
            _scopeMock.Setup(x => x.IsFunctionExist("print")).Returns(true);
            Parse("(func () print)", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 4, TokenKind.Identificator),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 1, TokenKind.CloseBracket),
                new Token(0, 9, 5, TokenKind.Identificator),
                new Token(0, 14, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Functions.Count);
            var func = _codeModel.Functions.First();
            Assert.AreEqual("func", func.Name);
            Assert.AreEqual(0, func.Arguments.Count);
            Assert.AreEqual(1, func.Body.Count);
            Assert.True(func.Body.First() is Function);
            var body = (Function)func.Body.First();
            Assert.AreEqual("print", body.Name);
            Assert.AreEqual(0, body.ArgumentValues.Count);
        }

        [Test]
        public void FunctionDeclaration_FunctionAlreadyExist()
        {
            _scopeMock.Setup(x => x.IsFunctionExist("func")).Returns(true);
            Parse("(func () print)", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 4, TokenKind.Identificator),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 1, TokenKind.CloseBracket),
                new Token(0, 9, 5, TokenKind.Identificator),
                new Token(0, 14, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(ParserMessageType.FunctionNameExist, _result.Error.Type);
            Assert.AreEqual(new Token(0, 1, 4, TokenKind.Identificator), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(0, _codeModel.Functions.Count);
        }

        [Test]
        public void FunctionDeclaration_ConstAlreadyExist()
        {
            _scopeMock.Setup(x => x.IsConstExist("func")).Returns(true);
            Parse("(func () print)", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 4, TokenKind.Identificator),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 1, TokenKind.CloseBracket),
                new Token(0, 9, 5, TokenKind.Identificator),
                new Token(0, 14, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(ParserMessageType.ConstNameExist, _result.Error.Type);
            Assert.AreEqual(new Token(0, 1, 4, TokenKind.Identificator), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(0, _codeModel.Functions.Count);
        }

        [Test]
        public void FunctionDeclaration_Variable()
        {
            Parse("(func (x) x)", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 4, TokenKind.Identificator),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 1, TokenKind.Identificator),
                new Token(0, 8, 1, TokenKind.CloseBracket),
                new Token(0, 10, 1, TokenKind.Identificator),
                new Token(0, 11, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Functions.Count);
            var func = _codeModel.Functions.First();
            Assert.AreEqual("func", func.Name);
            Assert.AreEqual(1, func.Arguments.Count);
            Assert.AreEqual(1, func.Body.Count);
            Assert.True(func.Body.First() is Variable);
            var body = (Variable)func.Body.First();
            Assert.AreEqual("x", body.Name);
        }

        [Test]
        public void FunctionDeclaration_OneFunctionCall()
        {
            _scopeMock.Setup(x => x.IsFunctionExist("g")).Returns(true);
            Parse("(f () (g 1 1.2 'str'))", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 1, TokenKind.Identificator),
                new Token(0, 3, 1, TokenKind.OpenBracket),
                new Token(0, 4, 1, TokenKind.CloseBracket),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 1, TokenKind.Identificator),
                new Token(0, 9, 1, TokenKind.IntegerNumber),
                new Token(0, 11, 3, TokenKind.FloatNumber),
                new Token(0, 15, 5, TokenKind.String),
                new Token(0, 20, 1, TokenKind.CloseBracket),
                new Token(0, 21, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Functions.Count);
            var func = _codeModel.Functions.First();
            Assert.AreEqual(1, func.Body.Count);
            Assert.True(func.Body.First() is Function);
            var body = (Function)func.Body.First();
            Assert.AreEqual("g", body.Name);
            Assert.AreEqual(3, body.ArgumentValues.Count);
            Assert.AreEqual(1, ((IntegerValue)body.ArgumentValues[0]).Value);
            Assert.AreEqual(1.2, ((FloatValue)body.ArgumentValues[1]).Value);
            Assert.AreEqual("str", ((StringValue)body.ArgumentValues[2]).Value);
        }

        [Test]
        public void FunctionDeclaration_InnerFunctionCall()
        {
            _scopeMock.Setup(x => x.IsFunctionExist("g")).Returns(true);
            _scopeMock.Setup(x => x.IsFunctionExist("k")).Returns(true);
            Parse("(f () (g 1 (k 1 2) 'str'))", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 1, TokenKind.Identificator),
                new Token(0, 3, 1, TokenKind.OpenBracket),
                new Token(0, 4, 1, TokenKind.CloseBracket),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 1, TokenKind.Identificator),
                new Token(0, 9, 1, TokenKind.IntegerNumber),
                new Token(0, 11, 1, TokenKind.OpenBracket),
                new Token(0, 12, 1, TokenKind.Identificator),
                new Token(0, 14, 1, TokenKind.IntegerNumber),
                new Token(0, 16, 1, TokenKind.IntegerNumber),
                new Token(0, 17, 1, TokenKind.CloseBracket),
                new Token(0, 19, 5, TokenKind.String),
                new Token(0, 24, 1, TokenKind.CloseBracket),
                new Token(0, 25, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Functions.Count);
            var func = _codeModel.Functions.First();
            Assert.AreEqual(1, func.Body.Count);
            Assert.True(func.Body.First() is Function);
            var body = (Function)func.Body.First();
            var innerbody = (Function)body.ArgumentValues[1];
            Assert.AreEqual("k", innerbody.Name);
            Assert.AreEqual(2, innerbody.ArgumentValues.Count);
            Assert.AreEqual(1, ((IntegerValue)innerbody.ArgumentValues[0]).Value);
            Assert.AreEqual(2, ((IntegerValue)innerbody.ArgumentValues[1]).Value);
        }

        [Test]
        public void FunctionDeclaration_Lambda()
        {
            _scopeMock.Setup(x => x.IsFunctionExist("g")).Returns(true);
            Parse("(f () (lambda (x) (g x)))", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 1, TokenKind.Identificator),
                new Token(0, 3, 1, TokenKind.OpenBracket),
                new Token(0, 4, 1, TokenKind.CloseBracket),
                new Token(0, 6, 1, TokenKind.OpenBracket),
                new Token(0, 7, 6, TokenKind.Lambda),
                new Token(0, 14, 1, TokenKind.OpenBracket),
                new Token(0, 15, 1, TokenKind.Identificator),
                new Token(0, 16, 1, TokenKind.CloseBracket),
                new Token(0, 18, 1, TokenKind.OpenBracket),
                new Token(0, 19, 1, TokenKind.Identificator),
                new Token(0, 21, 1, TokenKind.Identificator),
                new Token(0, 22, 1, TokenKind.CloseBracket),
                new Token(0, 23, 1, TokenKind.CloseBracket),
                new Token(0, 24, 1, TokenKind.CloseBracket),
            });

            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Functions.Count);
            var func = _codeModel.Functions.First();
            Assert.AreEqual(1, func.Body.Count);
            Assert.True(func.Body.First() is Lambda);
            var body = (Lambda)func.Body.First();
            Assert.AreEqual(1, body.Arguments.Count);

            Assert.AreEqual(1, func.Body.Count);
            Assert.True(body.Body.First() is Function);
            var lambdaBody = (Function)body.Body.First();
            Assert.AreEqual("g", lambdaBody.Name);
            Assert.AreEqual(1, lambdaBody.ArgumentValues.Count);
            Assert.AreEqual("x", ((Variable)lambdaBody.ArgumentValues[0]).Name);
        }

        [Test]
        public void FunctionDeclaration_TwoFunctionsInBody()
        {
            Parse("( f () (g 1) (h 2) )", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 2, 1, TokenKind.Identificator),
                new Token(0, 4, 1, TokenKind.OpenBracket),
                new Token(0, 5, 1, TokenKind.CloseBracket),
                new Token(0, 7, 1, TokenKind.OpenBracket),
                new Token(0, 8, 1, TokenKind.Identificator),
                new Token(0, 10, 1, TokenKind.IntegerNumber),
                new Token(0, 11, 1, TokenKind.CloseBracket),
                new Token(0, 13, 1, TokenKind.OpenBracket),
                new Token(0, 14, 1, TokenKind.Identificator),
                new Token(0, 16, 1, TokenKind.IntegerNumber),
                new Token(0, 17, 1, TokenKind.CloseBracket),
                new Token(0, 19, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.AreEqual(1, _codeModel.Functions.Count);
            var func = _codeModel.Functions.First();
            Assert.AreEqual(2, func.Body.Count);
        }

        [Test]
        public void RunFunctionCall()
        {
            _scopeMock.Setup(x => x.IsFunctionExist("f")).Returns(true);
            Parse("(run (f 1))", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 3, TokenKind.RunFunction),
                new Token(0, 5, 1, TokenKind.OpenBracket),
                new Token(0, 6, 1, TokenKind.Identificator),
                new Token(0, 8, 1, TokenKind.IntegerNumber),
                new Token(0, 9, 1, TokenKind.CloseBracket),
                new Token(0, 10, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(null, _result.Error);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.NotNull(_codeModel.RunFunction);
            var run = _codeModel.RunFunction!;
            Assert.AreEqual("f", run.Name);
            Assert.AreEqual(1, run.ArgumentValues.Count);
            Assert.AreEqual(typeof(IntegerValue), run.ArgumentValues[0].GetType());
        }

        [Test]
        public void RunFunctionExist()
        {
            _scopeMock.Setup(x => x.IsFunctionExist("f")).Returns(true);
            _scopeMock.Setup(x => x.IsRunFunctionExist()).Returns(true);
            Parse("(run (f 1))", new[]
            {
                new Token(0, 0, 1, TokenKind.OpenBracket),
                new Token(0, 1, 3, TokenKind.RunFunction),
                new Token(0, 5, 1, TokenKind.OpenBracket),
                new Token(0, 6, 1, TokenKind.Identificator),
                new Token(0, 8, 1, TokenKind.IntegerNumber),
                new Token(0, 9, 1, TokenKind.CloseBracket),
                new Token(0, 10, 1, TokenKind.CloseBracket),
            });
            Assert.AreEqual(ParserMessageType.RunFunctionExist, _result.Error.Type);
            Assert.AreEqual(new Token(0, 1, 3, TokenKind.RunFunction), _result.Error.Token);
            Assert.AreEqual(0, _result.Warnings.Count);
            Assert.IsNull(_codeModel.RunFunction);
        }

        private void Parse(string text, IEnumerable<Token> tokens)
        {
            _parser = new FunctionParser(new Text(text), new TokenIterator(tokens), _codeModel, _scopeMock.Object);
            _result = _parser.Parse();
        }
    }
}
