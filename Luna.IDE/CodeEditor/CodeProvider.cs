using System.Collections.Generic;
using System.Linq;
using CodeHighlighter;
using TokenKind = Luna.Parsing.TokenKind;

namespace Luna.IDE.CodeEditor;

public class CodeProvider : ICodeProvider
{
    public IEnumerable<Token> GetTokens(ITextIterator textIterator)
    {
        var scanner = new Parsing.Scanner();
        var tokens = scanner.GetTokens(new TextIteratorWrapper(textIterator)).Select(Convert).ToList();

        return tokens;
    }

    private Token Convert(Parsing.Token token)
    {
        return new Token(token.LineIndex, token.StartColumnIndex, token.Length, (byte)token.Kind);
    }

    public IEnumerable<TokenColor> GetColors()
    {
        return new[]
        {
            new TokenColor((byte) TokenKind.ImportDirective, CodeProviderColors.Magenta),
            new TokenColor((byte) TokenKind.Lambda, CodeProviderColors.Magenta),
            new TokenColor((byte) TokenKind.RunFunction, CodeProviderColors.Green),
            new TokenColor((byte) TokenKind.ConstDeclaration, CodeProviderColors.Blue),
            new TokenColor((byte) TokenKind.String, CodeProviderColors.Orange),
            new TokenColor((byte) TokenKind.IntegerNumber, CodeProviderColors.Red),
            new TokenColor((byte) TokenKind.FloatNumber, CodeProviderColors.Red),
            new TokenColor((byte) TokenKind.Comment, CodeProviderColors.LightGreen),
            new TokenColor((byte) TokenKind.OpenBracket, CodeProviderColors.Gray),
            new TokenColor((byte) TokenKind.CloseBracket, CodeProviderColors.Gray),
            new TokenColor((byte) TokenKind.Dot, CodeProviderColors.Gray),
        };
    }
}
