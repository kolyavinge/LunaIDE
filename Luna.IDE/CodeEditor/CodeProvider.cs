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
        return new(token.Name, token.LineIndex, token.StartColumnIndex, token.Length, (byte)token.Kind);
    }

    public IEnumerable<TokenColor> GetColors()
    {
        return new TokenColor[]
        {
            new((byte) TokenKind.ImportDirective, CodeProviderColors.Magenta),
            new((byte) TokenKind.Lambda, CodeProviderColors.Magenta),
            new((byte) TokenKind.RunFunction, CodeProviderColors.Green),
            new((byte) TokenKind.ConstDeclaration, CodeProviderColors.Blue),
            new((byte) TokenKind.String, CodeProviderColors.Orange),
            new((byte) TokenKind.IntegerNumber, CodeProviderColors.Red),
            new((byte) TokenKind.FloatNumber, CodeProviderColors.Red),
            new((byte) TokenKind.BooleanTrue, CodeProviderColors.Magenta),
            new((byte) TokenKind.BooleanFalse, CodeProviderColors.Magenta),
            new((byte) TokenKind.Comment, CodeProviderColors.LightGreen),
            new((byte) TokenKind.OpenBracket, CodeProviderColors.Gray),
            new((byte) TokenKind.CloseBracket, CodeProviderColors.Gray),
            new((byte) TokenKind.Dot, CodeProviderColors.Gray)
        };
    }
}
