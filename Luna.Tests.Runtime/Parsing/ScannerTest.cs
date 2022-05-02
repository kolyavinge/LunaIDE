using System.Collections.Generic;
using System.Linq;
using Luna.Parsing;
using NUnit.Framework;

namespace Luna.Tests.Parsing
{
    public class ScannerTest
    {
        private Scanner _scanner;

        [SetUp]
        public void Setup()
        {
            _scanner = new Scanner();
        }

        [Test]
        public void ImportDirective()
        {
            var tokens = GetTokens("import 'file path'");
            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(new Token(0, 0, 6, TokenKind.ImportDirective), tokens[0]);
            Assert.AreEqual(new Token(0, 7, 11, TokenKind.String), tokens[1]);
        }

        [Test]
        public void Const_IntegerNumber()
        {
            var tokens = GetTokens("const VALUE 123");
            Assert.AreEqual(3, tokens.Count);
            Assert.AreEqual(new Token(0, 0, 5, TokenKind.ConstDeclare), tokens[0]);
            Assert.AreEqual(new Token(0, 6, 5, TokenKind.Identificator), tokens[1]);
            Assert.AreEqual(new Token(0, 12, 3, TokenKind.IntegerNumber), tokens[2]);
        }

        [Test]
        public void Const_FloatNumber()
        {
            var tokens = GetTokens("const VALUE 1.23");
            Assert.AreEqual(3, tokens.Count);
            Assert.AreEqual(new Token(0, 0, 5, TokenKind.ConstDeclare), tokens[0]);
            Assert.AreEqual(new Token(0, 6, 5, TokenKind.Identificator), tokens[1]);
            Assert.AreEqual(new Token(0, 12, 4, TokenKind.FloatNumber), tokens[2]);
        }

        [Test]
        public void Const_Comment()
        {
            var tokens = GetTokens("// comment\r\n");
            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual(new Token(0, 0, 10, TokenKind.Comment), tokens[0]);
        }

        [Test]
        public void Const_CommentInFunction()
        {
            var tokens = GetTokens("(func (x//))");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual(new Token(0, 0, 1, TokenKind.OpenBracket), tokens[0]);
            Assert.AreEqual(new Token(0, 1, 4, TokenKind.Identificator), tokens[1]);
            Assert.AreEqual(new Token(0, 6, 1, TokenKind.OpenBracket), tokens[2]);
            Assert.AreEqual(new Token(0, 7, 1, TokenKind.Identificator), tokens[3]);
            Assert.AreEqual(new Token(0, 8, 4, TokenKind.Comment), tokens[4]);
        }

        [Test]
        public void Const_CommentAfterIntegerNumber()
        {
            var tokens = GetTokens("12//");
            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(new Token(0, 0, 2, TokenKind.IntegerNumber), tokens[0]);
            Assert.AreEqual(new Token(0, 2, 2, TokenKind.Comment), tokens[1]);
        }

        [Test]
        public void Const_CommentAfterFloatNumber()
        {
            var tokens = GetTokens("12.0//");
            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(new Token(0, 0, 4, TokenKind.FloatNumber), tokens[0]);
            Assert.AreEqual(new Token(0, 4, 2, TokenKind.Comment), tokens[1]);
        }

        [Test]
        public void ComplexIdentificator_1()
        {
            var tokens = GetTokens("xxx.yyy.zzz");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual(new Token(0, 0, 3, TokenKind.Identificator), tokens[0]);
            Assert.AreEqual(new Token(0, 3, 1, TokenKind.Dot), tokens[1]);
            Assert.AreEqual(new Token(0, 4, 3, TokenKind.Identificator), tokens[2]);
            Assert.AreEqual(new Token(0, 7, 1, TokenKind.Dot), tokens[3]);
            Assert.AreEqual(new Token(0, 8, 3, TokenKind.Identificator), tokens[4]);
        }

        [Test]
        public void FunctionDeclare()
        {
            var tokens = GetTokens("(func (x y z) (funcCall 1 2))");
            Assert.AreEqual(13, tokens.Count);
            Assert.AreEqual(new Token(0, 0, 1, TokenKind.OpenBracket), tokens[0]);
            Assert.AreEqual(new Token(0, 1, 4, TokenKind.Identificator), tokens[1]);
            Assert.AreEqual(new Token(0, 6, 1, TokenKind.OpenBracket), tokens[2]);
            Assert.AreEqual(new Token(0, 7, 1, TokenKind.Identificator), tokens[3]);
            Assert.AreEqual(new Token(0, 9, 1, TokenKind.Identificator), tokens[4]);
            Assert.AreEqual(new Token(0, 11, 1, TokenKind.Identificator), tokens[5]);
            Assert.AreEqual(new Token(0, 12, 1, TokenKind.CloseBracket), tokens[6]);
            Assert.AreEqual(new Token(0, 14, 1, TokenKind.OpenBracket), tokens[7]);
            Assert.AreEqual(new Token(0, 15, 8, TokenKind.Identificator), tokens[8]);
            Assert.AreEqual(new Token(0, 24, 1, TokenKind.IntegerNumber), tokens[9]);
            Assert.AreEqual(new Token(0, 26, 1, TokenKind.IntegerNumber), tokens[10]);
            Assert.AreEqual(new Token(0, 27, 1, TokenKind.CloseBracket), tokens[11]);
            Assert.AreEqual(new Token(0, 28, 1, TokenKind.CloseBracket), tokens[12]);
        }

        [Test]
        public void RunFunction()
        {
            var tokens = GetTokens("(run (myfunc))");
            Assert.AreEqual(6, tokens.Count);
            Assert.AreEqual(new Token(0, 0, 1, TokenKind.OpenBracket), tokens[0]);
            Assert.AreEqual(new Token(0, 1, 3, TokenKind.RunFunction), tokens[1]);
            Assert.AreEqual(new Token(0, 5, 1, TokenKind.OpenBracket), tokens[2]);
            Assert.AreEqual(new Token(0, 6, 6, TokenKind.Identificator), tokens[3]);
            Assert.AreEqual(new Token(0, 12, 1, TokenKind.CloseBracket), tokens[4]);
            Assert.AreEqual(new Token(0, 13, 1, TokenKind.CloseBracket), tokens[5]);
        }

        private List<Token> GetTokens(string text)
        {
            return _scanner.GetTokens(new TextIterator(new Text(text))).ToList();
        }
    }
}
