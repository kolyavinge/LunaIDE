using System;
using System.Collections.Generic;
using System.Text;

namespace Luna.Parsing
{
    public struct Lexem
    {
        public readonly int LineIndex;
        public readonly int StartColumnIndex;
        public readonly int Length;
        public readonly LexemKind Kind;

        public Lexem(int lineIndex, int startColumnIndex, int length, LexemKind kind)
        {
            LineIndex = lineIndex;
            StartColumnIndex = startColumnIndex;
            Length = length;
            Kind = kind;
        }
    }
}
