using CodeHighlighter;
using Luna.IDE.CodeEditor;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using LexemKind = Luna.Parsing.LexemKind;

namespace Luna.Tests.IDE.CodeEditor
{
    public class CodeProviderTest
    {
        private CodeProvider _provider;

        [SetUp]
        public void Setup()
        {
            _provider = new CodeProvider();
        }

        [Test]
        public void ImportDirective()
        {
            var lexems = GetLexems("import 'file path'");
            Assert.AreEqual(2, lexems.Count);
            Assert.AreEqual(new Lexem(0, 0, 6, (byte)LexemKind.ImportDirective), lexems[0]);
            Assert.AreEqual(new Lexem(0, 7, 11, (byte)LexemKind.String), lexems[1]);
        }

        [Test]
        public void Const_IntegerNumber()
        {
            var lexems = GetLexems("const VALUE 123");
            Assert.AreEqual(3, lexems.Count);
            Assert.AreEqual(new Lexem(0, 0, 5, (byte)LexemKind.ConstDeclare), lexems[0]);
            Assert.AreEqual(new Lexem(0, 6, 5, (byte)LexemKind.Identificator), lexems[1]);
            Assert.AreEqual(new Lexem(0, 12, 3, (byte)LexemKind.IntegerNumber), lexems[2]);
        }

        [Test]
        public void Const_FloatNumber()
        {
            var lexems = GetLexems("const VALUE 1.23");
            Assert.AreEqual(3, lexems.Count);
            Assert.AreEqual(new Lexem(0, 0, 5, (byte)LexemKind.ConstDeclare), lexems[0]);
            Assert.AreEqual(new Lexem(0, 6, 5, (byte)LexemKind.Identificator), lexems[1]);
            Assert.AreEqual(new Lexem(0, 12, 4, (byte)LexemKind.FloatNumber), lexems[2]);
        }

        [Test]
        public void Const_Comment()
        {
            var lexems = GetLexems("// comment\r\n");
            Assert.AreEqual(1, lexems.Count);
            Assert.AreEqual(new Lexem(0, 0, 10, (byte)LexemKind.Comment), lexems[0]);
        }

        [Test]
        public void Const_CommentInFunction()
        {
            var lexems = GetLexems("(func (x//))");
            Assert.AreEqual(5, lexems.Count);
            Assert.AreEqual(new Lexem(0, 0, 1, (byte)LexemKind.OpenBracket), lexems[0]);
            Assert.AreEqual(new Lexem(0, 1, 4, (byte)LexemKind.Identificator), lexems[1]);
            Assert.AreEqual(new Lexem(0, 6, 1, (byte)LexemKind.OpenBracket), lexems[2]);
            Assert.AreEqual(new Lexem(0, 7, 1, (byte)LexemKind.Identificator), lexems[3]);
            Assert.AreEqual(new Lexem(0, 8, 4, (byte)LexemKind.Comment), lexems[4]);
        }

        [Test]
        public void Const_CommentAfterIntegerNumber()
        {
            var lexems = GetLexems("12//");
            Assert.AreEqual(2, lexems.Count);
            Assert.AreEqual(new Lexem(0, 0, 2, (byte)LexemKind.IntegerNumber), lexems[0]);
            Assert.AreEqual(new Lexem(0, 2, 2, (byte)LexemKind.Comment), lexems[1]);
        }

        [Test]
        public void Const_CommentAfterFloatNumber()
        {
            var lexems = GetLexems("12.0//");
            Assert.AreEqual(2, lexems.Count);
            Assert.AreEqual(new Lexem(0, 0, 4, (byte)LexemKind.FloatNumber), lexems[0]);
            Assert.AreEqual(new Lexem(0, 4, 2, (byte)LexemKind.Comment), lexems[1]);
        }

        [Test]
        public void ComplexIdentificator()
        {
            var lexems = GetLexems("x.y.z");
            Assert.AreEqual(5, lexems.Count);
            Assert.AreEqual(new Lexem(0, 0, 1, (byte)LexemKind.Identificator), lexems[0]);
            Assert.AreEqual(new Lexem(0, 1, 1, (byte)LexemKind.Dot), lexems[1]);
            Assert.AreEqual(new Lexem(0, 2, 1, (byte)LexemKind.Identificator), lexems[2]);
            Assert.AreEqual(new Lexem(0, 3, 1, (byte)LexemKind.Dot), lexems[3]);
            Assert.AreEqual(new Lexem(0, 4, 1, (byte)LexemKind.Identificator), lexems[4]);
        }

        [Test]
        public void FunctionDeclare()
        {
            var lexems = GetLexems("(func (x y z) (funcCall 1 2))");
            Assert.AreEqual(13, lexems.Count);
            Assert.AreEqual(new Lexem(0, 0, 1, (byte)LexemKind.OpenBracket), lexems[0]);
            Assert.AreEqual(new Lexem(0, 1, 4, (byte)LexemKind.Identificator), lexems[1]);
            Assert.AreEqual(new Lexem(0, 6, 1, (byte)LexemKind.OpenBracket), lexems[2]);
            Assert.AreEqual(new Lexem(0, 7, 1, (byte)LexemKind.Identificator), lexems[3]);
            Assert.AreEqual(new Lexem(0, 9, 1, (byte)LexemKind.Identificator), lexems[4]);
            Assert.AreEqual(new Lexem(0, 11, 1, (byte)LexemKind.Identificator), lexems[5]);
            Assert.AreEqual(new Lexem(0, 12, 1, (byte)LexemKind.CloseBracket), lexems[6]);
            Assert.AreEqual(new Lexem(0, 14, 1, (byte)LexemKind.OpenBracket), lexems[7]);
            Assert.AreEqual(new Lexem(0, 15, 8, (byte)LexemKind.Identificator), lexems[8]);
            Assert.AreEqual(new Lexem(0, 24, 1, (byte)LexemKind.IntegerNumber), lexems[9]);
            Assert.AreEqual(new Lexem(0, 26, 1, (byte)LexemKind.IntegerNumber), lexems[10]);
            Assert.AreEqual(new Lexem(0, 27, 1, (byte)LexemKind.CloseBracket), lexems[11]);
            Assert.AreEqual(new Lexem(0, 28, 1, (byte)LexemKind.CloseBracket), lexems[12]);
        }

        [Test]
        public void RunFunction()
        {
            var lexems = GetLexems("(run (myfunc))");
            Assert.AreEqual(6, lexems.Count);
            Assert.AreEqual(new Lexem(0, 0, 1, (byte)LexemKind.OpenBracket), lexems[0]);
            Assert.AreEqual(new Lexem(0, 1, 3, (byte)LexemKind.RunFunction), lexems[1]);
            Assert.AreEqual(new Lexem(0, 5, 1, (byte)LexemKind.OpenBracket), lexems[2]);
            Assert.AreEqual(new Lexem(0, 6, 6, (byte)LexemKind.Identificator), lexems[3]);
            Assert.AreEqual(new Lexem(0, 12, 1, (byte)LexemKind.CloseBracket), lexems[4]);
            Assert.AreEqual(new Lexem(0, 13, 1, (byte)LexemKind.CloseBracket), lexems[5]);
        }

        private List<Lexem> GetLexems(string text)
        {
            return _provider.GetLexems(TextIteratorBuilder.FromString(text)).ToList();
        }
    }
}
