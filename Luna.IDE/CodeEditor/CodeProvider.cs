using System.Collections.Generic;
using System.Linq;
using CodeHighlighter;
using LexemKind = Luna.Parsing.LexemKind;

namespace Luna.IDE.CodeEditor
{
    public class CodeProvider : ICodeProvider
    {
        public IEnumerable<Lexem> GetLexems(ITextIterator textIterator)
        {
            var scanner = new Parsing.Scanner();
            var lexems = scanner.GetLexems(new TextIteratorWrapper(textIterator)).Select(Convert).ToList();

            return lexems;
        }

        private Lexem Convert(Parsing.Lexem lexem)
        {
            return new Lexem(lexem.LineIndex, lexem.StartColumnIndex, lexem.Length, (byte)lexem.Kind);
        }

        public IEnumerable<LexemColor> GetColors()
        {
            return new[]
            {
                new LexemColor((byte) LexemKind.ImportDirective, CodeProviderColors.Magenta),
                new LexemColor((byte) LexemKind.LambdaIdentificator, CodeProviderColors.Magenta),
                new LexemColor((byte) LexemKind.RunFunction, CodeProviderColors.Green),
                new LexemColor((byte) LexemKind.ConstDeclare, CodeProviderColors.Blue),
                new LexemColor((byte) LexemKind.String, CodeProviderColors.Orange),
                new LexemColor((byte) LexemKind.IntegerNumber, CodeProviderColors.Red),
                new LexemColor((byte) LexemKind.FloatNumber, CodeProviderColors.Red),
                new LexemColor((byte) LexemKind.Comment, CodeProviderColors.LightGreen),
                new LexemColor((byte) LexemKind.OpenBracket, CodeProviderColors.Gray),
                new LexemColor((byte) LexemKind.CloseBracket, CodeProviderColors.Gray),
                new LexemColor((byte) LexemKind.Dot, CodeProviderColors.Gray),
            };
        }
    }
}
