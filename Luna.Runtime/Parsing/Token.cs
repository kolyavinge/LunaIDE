using System;

namespace Luna.Parsing;

public class Token
{
    public static readonly Token Default = new("", 0, 0, 0, TokenKind.Unknown);

    public readonly string Name;
    public readonly int LineIndex;
    public readonly int StartColumnIndex;
    public readonly int Length;
    public readonly TokenKind Kind;

    public Token(string name, int lineIndex, int startColumnIndex, int length, TokenKind kind)
    {
        Name = name;
        LineIndex = lineIndex;
        StartColumnIndex = startColumnIndex;
        Length = length;
        Kind = kind;
    }

    public override bool Equals(object? obj)
    {
        return obj is Token token &&
               Name == token.Name &&
               LineIndex == token.LineIndex &&
               StartColumnIndex == token.StartColumnIndex &&
               Length == token.Length &&
               Kind == token.Kind;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, LineIndex, StartColumnIndex, Length, Kind);
    }
}

public enum TokenKind : byte
{
    Unknown,
    ImportDirective,
    ConstDeclaration,
    OpenBracket,
    CloseBracket,
    Plus,
    Minus,
    Asterisk,
    Slash,
    Percent,
    String,
    Identificator,
    Variable,
    Lambda,
    RunFunction,
    IntegerNumber,
    FloatNumber,
    BooleanTrue,
    BooleanFalse,
    Comment,
    Dot,
    Colon
}
