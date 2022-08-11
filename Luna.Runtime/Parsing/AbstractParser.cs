using System;
using System.Collections.Generic;
using System.Globalization;
using Luna.ProjectModel;

namespace Luna.Parsing;

public class ParseResult
{
    private readonly List<ParserMessage> _errors = new();
    private readonly List<ParserMessage> _warnings = new();

    public IReadOnlyCollection<ParserMessage> Errors => _errors;
    public IReadOnlyCollection<ParserMessage> Warnings => _warnings;

    internal void AddError(ParserMessage error)
    {
        _errors.Add(error);
    }

    internal void AddWarning(ParserMessage warning)
    {
        _warnings.Add(warning);
    }
}

public abstract class AbstractParser
{
    private readonly TokenIterator _iter;
    protected readonly CodeModel _codeModel;
    protected readonly ParseResult _result;

    protected Token Prev => _iter.PrevToken;
    protected Token Token => _iter.Token;
    protected bool Eof => _iter.Eof;

    protected int BracketCorrespondence { get; private set; }

    protected AbstractParser(TokenIterator iter, CodeModel codeModel)
    {
        _iter = iter;
        _codeModel = codeModel;
        _result = new();
        SetBracketCorrespondence();
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
            if (Token.Name.Length > 1 && Token.Name.EndsWith("'")) return Token.Name.Substring(1, Token.Length - 2);
            else return Token.Name[1..Token.Length];
        }
        else
        {
            return Token.Name;
        }
    }

    protected bool TryParseLongValue(out long result)
    {
        var stringValue = GetTokenName();
        return Int64.TryParse(stringValue, out result);
    }

    protected bool TryParseDoubleValue(out double result)
    {
        var stringValue = GetTokenName();
        return Double.TryParse(stringValue, NumberStyles.Any, new NumberFormatInfo { NumberDecimalSeparator = "." }, out result);
    }

    protected void MoveNext()
    {
        _iter.MoveNext();
        SetBracketCorrespondence();
    }

    protected void SkipLine(int lineIndex)
    {
        while (!_iter.Eof && lineIndex == Token.LineIndex)
        {
            MoveNext();
        }
    }

    protected void SkipFunctionDeclaration()
    {
        while (!Eof && BracketCorrespondence != 0) MoveNext();
        MoveNext();
    }

    private void SetBracketCorrespondence()
    {
        if (Token.Kind == TokenKind.OpenBracket) BracketCorrespondence++;
        else if (Token.Kind == TokenKind.CloseBracket) BracketCorrespondence--;
    }
}
