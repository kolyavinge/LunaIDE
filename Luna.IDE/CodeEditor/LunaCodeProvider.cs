using System;
using System.Collections.Generic;
using System.Linq;
using CodeHighlighter.CodeProvidering;
using TokenKind = Luna.Parsing.TokenKind;

namespace Luna.IDE.CodeEditor;

public interface ILunaCodeProvider : ICodeProvider, ITokenKindUpdatable
{
    void UpdateTokenKinds(IEnumerable<UpdatedTokenKind> updatedTokens);
}

public class LunaCodeProvider : ILunaCodeProvider
{
    private readonly ICodeProviderScope _scope;

    public event EventHandler<TokenKindUpdatedEventArgs>? TokenKindUpdated;

    public LunaCodeProvider(ICodeProviderScope scope)
    {
        _scope = scope;
    }

    public IEnumerable<Token> GetTokens(ITextIterator textIterator)
    {
        var scanner = new Parsing.Scanner();
        var tokens = scanner.GetTokens(new TextIteratorWrapper(textIterator)).Select(Convert).ToList();

        return tokens;
    }

    private Token Convert(Parsing.Token token)
    {
        var kind = GetTokenKind(token.Name, token.Kind);
        return new(token.Name, token.LineIndex, token.StartColumnIndex, token.Length, kind);
    }

    private byte GetTokenKind(string name, TokenKind kind)
    {
        if (_scope.IsFunction(name)) return (byte)TokenKindExtra.Function;
        if (_scope.IsConstant(name)) return (byte)TokenKindExtra.Constant;

        return (byte)kind;
    }

    public IEnumerable<TokenColor> GetColors()
    {
        return new TokenColor[]
        {
            new((byte) TokenKind.ImportDirective, CodeProviderColors.Blue),
            new((byte) TokenKind.ConstDeclaration, CodeProviderColors.Blue),
            new((byte) TokenKind.OpenBracket, CodeProviderColors.Gray),
            new((byte) TokenKind.CloseBracket, CodeProviderColors.Gray),
            new((byte) TokenKind.String, CodeProviderColors.Orange),
            new((byte) TokenKind.Lambda, CodeProviderColors.Magenta),
            new((byte) TokenKind.RunFunction, CodeProviderColors.Green),
            new((byte) TokenKind.IntegerNumber, CodeProviderColors.Red),
            new((byte) TokenKind.FloatNumber, CodeProviderColors.Red),
            new((byte) TokenKind.BooleanTrue, CodeProviderColors.Magenta),
            new((byte) TokenKind.BooleanFalse, CodeProviderColors.Magenta),
            new((byte) TokenKind.Comment, CodeProviderColors.Gray),
            new((byte) TokenKind.Dot, CodeProviderColors.Gray),
            new((byte) TokenKind.Colon, CodeProviderColors.Gray),
            new((byte) TokenKind.Variable, CodeProviderColors.Purple),
            new((byte) TokenKindExtra.Constant, CodeProviderColors.LightBlue),
            new((byte) TokenKindExtra.Function, CodeProviderColors.Yellow)
        };
    }

    public void UpdateTokenKinds(IEnumerable<UpdatedTokenKind> updatedTokens)
    {
        TokenKindUpdated?.Invoke(this, new(updatedTokens));
    }
}

public enum TokenKindExtra : byte
{
    Constant = 254,
    Function = 255
}
