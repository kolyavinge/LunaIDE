using System.Collections.Generic;
using System.Linq;

namespace Luna.Parsing
{
    public interface ITextIterator
    {
        char Char { get; }
        char NextChar { get; }
        int LineIndex { get; }
        int ColumnIndex { get; }
        bool Eof { get; }
        void MoveNext();
    }

    internal class TextIterator : ITextIterator
    {
        private readonly Text _text;

        public char Char { get; private set; }

        public int LineIndex { get; private set; }

        public int ColumnIndex { get; private set; }

        public bool Eof { get; private set; }

        public char NextChar
        {
            get
            {
                if (Eof) return (char)0;
                if (Char == '\n') return (char)0;
                var line = _text.GetLine(LineIndex);
                if (ColumnIndex == line.Length - 1 && LineIndex == _text.LinesCount - 1) return (char)0;
                if (ColumnIndex == line.Length - 1) return '\n';
                return line[ColumnIndex + 1];
            }
        }

        public TextIterator(Text text)
        {
            _text = text;
            LineIndex = 0;
            ColumnIndex = -1;
            MoveNext();
        }

        public void MoveNext()
        {
            if (Eof) return;
            if (Char == '\n')
            {
                ColumnIndex = 0;
                LineIndex++;
            }
            else
            {
                ColumnIndex++;
            }
            var line = _text.GetLine(LineIndex);
            if (ColumnIndex < line.Length)
            {
                Char = line[ColumnIndex];
            }
            else if (ColumnIndex == line.Length && LineIndex < _text.LinesCount - 1)
            {
                Char = '\n';
            }
            else
            {
                Char = (char)0;
                Eof = true;
            }
        }
    }
}
