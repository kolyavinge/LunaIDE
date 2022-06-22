using System;
using System.Collections.Generic;
using Luna.Utils;

namespace Luna.Parsing;

public class Scanner
{
    private ITextIterator? _textIterator;
    private readonly List<Token> _tokens = new();
    private int _lineIndex;
    private int _columnIndex;
    private int _nameLength;
    private readonly char[] _nameArray = new char[1024];
    private TokenKind? _kind;

    public IEnumerable<Token> GetTokens(ITextIterator textIterator)
    {
        _textIterator = textIterator;
        _tokens.Clear();
        switch (State.Begin)
        {
            case State.Begin:
                if (textIterator.Eof) { goto case State.End; }
                else if (IsSpace()) { textIterator.MoveNext(); goto case State.Begin; }
                else if (IsReturn()) { textIterator.MoveNext(); goto case State.Begin; }
                else if (textIterator.Char == '(')
                {
                    _lineIndex = textIterator.LineIndex;
                    _columnIndex = textIterator.ColumnIndex;
                    _nameLength = 0;
                    _kind = TokenKind.OpenBracket;
                    AddTokenChar();
                    MakeToken();
                    textIterator.MoveNext();
                    goto case State.Begin;
                }
                else if (textIterator.Char == ')')
                {
                    _lineIndex = textIterator.LineIndex;
                    _columnIndex = textIterator.ColumnIndex;
                    _nameLength = 0;
                    _kind = TokenKind.CloseBracket;
                    AddTokenChar();
                    MakeToken();
                    textIterator.MoveNext();
                    goto case State.Begin;
                }
                else if (textIterator.Char == '\'')
                {
                    _lineIndex = textIterator.LineIndex;
                    _columnIndex = textIterator.ColumnIndex;
                    _nameLength = 0;
                    _kind = TokenKind.String;
                    AddTokenChar();
                    textIterator.MoveNext();
                    goto case State.String;
                }
                else if (IsStartIdentificatorNameChar())
                {
                    _lineIndex = textIterator.LineIndex;
                    _columnIndex = textIterator.ColumnIndex;
                    _nameLength = 0;
                    AddTokenChar();
                    textIterator.MoveNext();
                    goto case State.Identificator;
                }
                else if (textIterator.Char == '+' && IsNextDigit())
                {
                    _lineIndex = textIterator.LineIndex;
                    _columnIndex = textIterator.ColumnIndex;
                    _nameLength = 0;
                    textIterator.MoveNext();
                    goto case State.Number;
                }
                else if (textIterator.Char == '-' && IsNextDigit())
                {
                    _lineIndex = textIterator.LineIndex;
                    _columnIndex = textIterator.ColumnIndex;
                    _nameLength = 0;
                    AddTokenChar();
                    textIterator.MoveNext();
                    goto case State.Number;
                }
                else if (IsDigit())
                {
                    _lineIndex = textIterator.LineIndex;
                    _columnIndex = textIterator.ColumnIndex;
                    _nameLength = 0;
                    AddTokenChar();
                    textIterator.MoveNext();
                    goto case State.Number;
                }
                else if (textIterator.Char == '/' && textIterator.NextChar == '/')
                {
                    _lineIndex = textIterator.LineIndex;
                    _columnIndex = textIterator.ColumnIndex;
                    _nameLength = 0;
                    _kind = TokenKind.Comment;
                    AddTokenChar();
                    textIterator.MoveNext();
                    goto case State.Comment;
                }
                else if (IsOperator())
                {
                    _lineIndex = textIterator.LineIndex;
                    _columnIndex = textIterator.ColumnIndex;
                    _nameLength = 0;
                    AddTokenChar();
                    MakeToken();
                    textIterator.MoveNext();
                    goto case State.Begin;
                }
                else if (_textIterator!.Char == '.')
                {
                    _lineIndex = textIterator.LineIndex;
                    _columnIndex = textIterator.ColumnIndex;
                    _nameLength = 0;
                    _kind = TokenKind.Dot;
                    AddTokenChar();
                    MakeToken();
                    textIterator.MoveNext();
                    goto case State.Begin;
                }
                else if (_textIterator!.Char == ':')
                {
                    _lineIndex = textIterator.LineIndex;
                    _columnIndex = textIterator.ColumnIndex;
                    _nameLength = 0;
                    _kind = TokenKind.Colon;
                    AddTokenChar();
                    MakeToken();
                    textIterator.MoveNext();
                    goto case State.Begin;
                }
                else
                {
                    _lineIndex = textIterator.LineIndex;
                    _columnIndex = textIterator.ColumnIndex;
                    _nameLength = 0;
                    goto case State.Error;
                }
            case State.Identificator:
                if (textIterator.Eof) { MakeToken(); goto case State.End; }
                else if (IsSpace()) { MakeToken(); textIterator.MoveNext(); goto case State.Begin; }
                else if (IsReturn()) { MakeToken(); textIterator.MoveNext(); goto case State.Begin; }
                else if (textIterator.Char == ')') { MakeToken(); goto case State.Begin; }
                else if (IsDelimiter()) { MakeToken(); goto case State.Begin; }
                else if (IsIdentificatorNameChar()) { AddTokenChar(); textIterator.MoveNext(); goto case State.Identificator; }
                else goto case State.Error;
            case State.String:
                if (textIterator.Eof) { MakeToken(); goto case State.End; }
                else if (IsReturn()) { MakeToken(); textIterator.MoveNext(); goto case State.Begin; }
                else if (textIterator.Char == '\'') { AddTokenChar(); MakeToken(); textIterator.MoveNext(); goto case State.Begin; }
                else { AddTokenChar(); textIterator.MoveNext(); goto case State.String; }
            case State.Number:
                if (textIterator.Eof) { _kind = TokenKind.IntegerNumber; MakeToken(); goto case State.End; }
                else if (IsSpace()) { _kind = TokenKind.IntegerNumber; MakeToken(); textIterator.MoveNext(); goto case State.Begin; }
                else if (IsReturn()) { _kind = TokenKind.IntegerNumber; MakeToken(); textIterator.MoveNext(); goto case State.Begin; }
                else if (textIterator.Char == '.') { _kind = TokenKind.FloatNumber; AddTokenChar(); textIterator.MoveNext(); goto case State.FloatNumber; }
                else if (IsDelimiter()) { _kind = TokenKind.IntegerNumber; MakeToken(); goto case State.Begin; }
                else if (textIterator.Char == ')') { _kind = TokenKind.IntegerNumber; MakeToken(); goto case State.Begin; }
                else if (IsDigit()) { _kind = TokenKind.IntegerNumber; AddTokenChar(); textIterator.MoveNext(); goto case State.Number; }
                else goto case State.Error;
            case State.FloatNumber:
                if (textIterator.Eof) { MakeToken(); goto case State.End; }
                else if (IsSpace()) { MakeToken(); textIterator.MoveNext(); goto case State.Begin; }
                else if (IsReturn()) { MakeToken(); textIterator.MoveNext(); goto case State.Begin; }
                else if (IsDelimiter()) { MakeToken(); goto case State.Begin; }
                else if (IsDigit()) { AddTokenChar(); textIterator.MoveNext(); goto case State.FloatNumber; }
                else goto case State.Error;
            case State.Comment:
                if (textIterator.Eof) { MakeToken(); goto case State.End; }
                else if (IsReturn()) { MakeToken(); textIterator.MoveNext(); goto case State.Begin; }
                else { AddTokenChar(); textIterator.MoveNext(); goto case State.Comment; }
            case State.Error:
                _kind = TokenKind.Unknown;
                AddTokenChar();
                MakeToken();
                textIterator.MoveNext();
                goto case State.Begin;
            case State.End:
                break;
        }

        return _tokens;
    }

    private void AddTokenChar()
    {
        _nameArray[_nameLength++] = _textIterator!.Char;
    }

    private void MakeToken()
    {
        _tokens.Add(new(new string(_nameArray, 0, _nameLength), _lineIndex, _columnIndex, _nameLength, _kind ?? GetTokenKind()));
        _kind = null;
    }

    private bool IsStartIdentificatorNameChar()
    {
        return Char.IsLetter(_textIterator!.Char) || _textIterator!.Char == '_';
    }

    private bool IsIdentificatorNameChar()
    {
        return Char.IsLetterOrDigit(_textIterator!.Char) || _textIterator!.Char == '_';
    }

    private bool IsDigit()
    {
        return Char.IsDigit(_textIterator!.Char);
    }

    private bool IsNextDigit()
    {
        return Char.IsDigit(_textIterator!.NextChar);
    }

    private bool IsOperator()
    {
        return _textIterator!.Char == '+' || _textIterator.Char == '-' || _textIterator.Char == '*' || _textIterator.Char == '/';
    }

    private bool IsDelimiter()
    {
        return _textIterator!.Char == '(' || _textIterator!.Char == ')' || _textIterator!.Char == '.' || _textIterator!.Char == ':' || IsOperator();
    }

    private bool IsSpace()
    {
        return _textIterator!.Char == ' ' || _textIterator!.Char == '\t';
    }

    private bool IsReturn()
    {
        return _textIterator!.Char == '\n';
    }

    private TokenKind GetTokenKind()
    {
        if (StringUtils.StringEquals("import", _nameArray, _nameLength)) return TokenKind.ImportDirective;
        if (StringUtils.StringEquals("const", _nameArray, _nameLength)) return TokenKind.ConstDeclaration;
        if (StringUtils.StringEquals("lambda", _nameArray, _nameLength)) return TokenKind.Lambda;
        if (StringUtils.StringEquals("true", _nameArray, _nameLength)) return TokenKind.BooleanTrue;
        if (StringUtils.StringEquals("false", _nameArray, _nameLength)) return TokenKind.BooleanFalse;
        if (StringUtils.StringEquals("run", _nameArray, _nameLength)) return TokenKind.RunFunction;
        if (StringUtils.StringEquals("+", _nameArray, _nameLength)) return TokenKind.Plus;
        if (StringUtils.StringEquals("-", _nameArray, _nameLength)) return TokenKind.Minus;
        if (StringUtils.StringEquals("*", _nameArray, _nameLength)) return TokenKind.Asterisk;
        if (StringUtils.StringEquals("/", _nameArray, _nameLength)) return TokenKind.Slash;

        return TokenKind.Identificator;
    }

    enum State
    {
        Begin,
        Identificator,
        Number,
        FloatNumber,
        String,
        Comment,
        Error,
        End
    }
}
