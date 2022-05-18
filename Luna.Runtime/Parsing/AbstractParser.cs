using System;
using System.Collections.Generic;
using System.Globalization;
using Luna.ProjectModel;

namespace Luna.Parsing;

public class ParseResult
{
    private readonly List<ParserMessage> _warnings = new();

    public ParserMessage? Error { get; private set; }
    public IReadOnlyCollection<ParserMessage> Warnings => _warnings;

    internal void SetError(ParserMessageType type, in Token token)
    {
        Error = new ParserMessage(type, in token);
    }

    internal void SetError(ParserMessageType type, IEnumerable<Token> tokens)
    {
        Error = new ParserMessage(type, tokens);
    }

    internal void AddWarning(ParserMessageType type, in Token token)
    {
        _warnings.Add(new(type, in token));
    }
}

public abstract class AbstractParser
{
    private readonly Text _text;
    private readonly TokenIterator _iter;
    protected readonly CodeModel _codeModel;
    protected readonly ParseResult _result;

    protected Token Prev => _iter.PrevToken;
    protected Token Token => _iter.Token;

    protected AbstractParser(Text text, TokenIterator iter, CodeModel codeModel)
    {
        _text = text;
        _iter = iter;
        _codeModel = codeModel;
        _result = new();
    }

    public ParseResult Parse()
    {
        InnerParse();
        return _result;
    }

    protected abstract void InnerParse();

    protected List<Token> GetRemainingTokens(int lineIndex)
    {
        var result = new List<Token>();
        while (!_iter.Eof && lineIndex == Token.LineIndex)
        {
            result.Add(Token);
            MoveNext();
        }

        return result;
    }

    protected string GetTokenName()
    {
        if (Token.Kind == TokenKind.String)
        {
            return _text.GetLine(Token.LineIndex).Substring(Token.StartColumnIndex + 1, Token.Length - 2);
        }
        else
        {
            return _text.GetLine(Token.LineIndex).Substring(Token.StartColumnIndex, Token.Length);
        }
    }

    protected int GetIntegerValue()
    {
        var stringValue = GetTokenName();
        return Int32.Parse(stringValue);
    }

    protected double GetDoubleValue()
    {
        var stringValue = GetTokenName();
        return Double.Parse(stringValue, new NumberFormatInfo { NumberDecimalSeparator = "." });
    }

    protected bool Eof => _iter.Eof;

    protected void MoveNext()
    {
        _iter.MoveNext();
    }
}
