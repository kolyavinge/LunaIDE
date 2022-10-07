using Luna.Parsing;

namespace Luna.IDE.CodeEditing;

public static class TokenExt
{
    public static bool IsIdentificator(this CodeHighlighter.Model.Token token)
    {
        return token.Kind is (byte)TokenKind.Identificator or (byte)TokenKindExtra.Constant or (byte)TokenKindExtra.Function;
    }

    public static bool IsKeyword(this CodeHighlighter.Model.Token token)
    {
        return token.Kind is
            (byte)TokenKind.BooleanFalse or
            (byte)TokenKind.BooleanTrue or
            (byte)TokenKind.ConstDeclaration or
            (byte)TokenKind.ImportDirective or
            (byte)TokenKind.Lambda or
            (byte)TokenKind.RunFunction;
    }

    public static bool IsOperator(this CodeHighlighter.Model.Token token)
    {
        return token.Kind is
            (byte)TokenKind.Plus or
            (byte)TokenKind.Minus or
            (byte)TokenKind.Asterisk or
            (byte)TokenKind.Slash or
            (byte)TokenKind.Percent;
    }
}
