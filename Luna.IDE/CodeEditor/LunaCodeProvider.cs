using System;
using System.Collections.Generic;
using System.Linq;
using CodeHighlighter;
using TokenKind = Luna.Parsing.TokenKind;

namespace Luna.IDE.CodeEditor;

public interface ILunaCodeProvider : ICodeProvider, ITokenKindUpdatable
{
    void UpdateIdentificators();
}

public class LunaCodeProvider : ILunaCodeProvider
{
    private readonly ICodeProviderScope _scope;
    private HashSet<string> _scannedIdentificators = new();

    public event EventHandler<TokenKindUpdatedEventArgs>? TokenKindUpdated;

    public LunaCodeProvider(ICodeProviderScope scope)
    {
        _scope = scope;
    }

    public IEnumerable<Token> GetTokens(ITextIterator textIterator)
    {
        var scanner = new Parsing.Scanner();
        var scannedTokens = scanner.GetTokens(new TextIteratorWrapper(textIterator)).ToList();
        _scannedIdentificators = scannedTokens.Where(x => x.Kind == TokenKind.Identificator).Select(x => x.Name).ToHashSet();
        var tokens = scannedTokens.Select(Convert).ToList();

        return tokens;
    }

    private Token Convert(Parsing.Token token)
    {
        var kind = _scope.IsFunction(token.Name) ? (byte)TokenKindExtra.Function : (byte)token.Kind;
        return new(token.Name, token.LineIndex, token.StartColumnIndex, token.Length, kind);
    }

    public IEnumerable<TokenColor> GetColors()
    {
        return new TokenColor[]
        {
            new((byte) TokenKind.ImportDirective, CodeProviderColors.Blue),
            new((byte) TokenKind.ConstDeclaration, CodeProviderColors.Blue),
            new((byte) TokenKind.OpenBracket, CodeProviderColors.Gray),
            new((byte) TokenKind.CloseBracket, CodeProviderColors.Gray),
            new((byte) TokenKind.Plus, CodeProviderColors.Yellow),
            new((byte) TokenKind.Minus, CodeProviderColors.Yellow),
            new((byte) TokenKind.Asterisk, CodeProviderColors.Yellow),
            new((byte) TokenKind.Slash, CodeProviderColors.Yellow),
            new((byte) TokenKind.String, CodeProviderColors.Orange),
            new((byte) TokenKind.Lambda, CodeProviderColors.Magenta),
            new((byte) TokenKind.RunFunction, CodeProviderColors.Green),
            new((byte) TokenKind.IntegerNumber, CodeProviderColors.Red),
            new((byte) TokenKind.FloatNumber, CodeProviderColors.Red),
            new((byte) TokenKind.BooleanTrue, CodeProviderColors.Magenta),
            new((byte) TokenKind.BooleanFalse, CodeProviderColors.Magenta),
            new((byte) TokenKind.Comment, CodeProviderColors.Gray),
            new((byte) TokenKind.Dot, CodeProviderColors.Gray),
            new((byte) TokenKindExtra.Function, CodeProviderColors.Yellow)
        };
    }

    public void UpdateIdentificators()
    {
        if (TokenKindUpdated == null) return;
        var functions = new List<UpdatedTokenKind>();
        var identificators = new List<UpdatedTokenKind>();
        foreach (var identificator in _scannedIdentificators)
        {
            if (_scope.IsFunction(identificator)) functions.Add(new(identificator, (byte)TokenKindExtra.Function));
            else identificators.Add(new(identificator, (byte)TokenKind.Identificator));
        }
        TokenKindUpdated.Invoke(this, new(functions.Concat(identificators)));
    }
}

public enum TokenKindExtra : byte
{
    Function = 255
}
