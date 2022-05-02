using System;
using System.Collections.Generic;
using System.Text;

namespace Luna.Parsing
{
    public struct Token
    {
        public readonly int LineIndex;
        public readonly int StartColumnIndex;
        public readonly int Length;
        public readonly TokenKind Kind;

        public Token(int lineIndex, int startColumnIndex, int length, TokenKind kind)
        {
            LineIndex = lineIndex;
            StartColumnIndex = startColumnIndex;
            Length = length;
            Kind = kind;
        }
    }

    public enum TokenKind : byte
    {
        Unknown,
        ImportDirective,
        ConstDeclaration,
        OpenBracket,
        CloseBracket,
        Operator,
        String,
        Identificator,
        Lambda,
        RunFunction,
        IntegerNumber,
        FloatNumber,
        Comment,
        Dot
    }
}
