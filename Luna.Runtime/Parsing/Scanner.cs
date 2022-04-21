using System;
using System.Collections.Generic;

namespace Luna.Parsing
{
    public class Scanner
    {
        private ITextIterator? _textIterator;
        private readonly List<Lexem> _lexems = new();
        private int _lineIndex;
        private int _columnIndex;
        private int _nameLength;
        private readonly char[] _nameArray = new char[1024];
        private LexemKind? _kind;

        public IEnumerable<Lexem> GetLexems(ITextIterator textIterator)
        {
            _textIterator = textIterator;
            _lexems.Clear();
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
                        _nameLength = 1;
                        _kind = LexemKind.OpenBracket;
                        MakeLexem();
                        textIterator.MoveNext();
                        goto case State.Begin;
                    }
                    else if (textIterator.Char == ')')
                    {
                        _lineIndex = textIterator.LineIndex;
                        _columnIndex = textIterator.ColumnIndex;
                        _nameLength = 1;
                        _kind = LexemKind.CloseBracket;
                        MakeLexem();
                        textIterator.MoveNext();
                        goto case State.Begin;
                    }
                    else if (textIterator.Char == '\'')
                    {
                        _lineIndex = textIterator.LineIndex;
                        _columnIndex = textIterator.ColumnIndex;
                        _nameLength = 1;
                        _kind = LexemKind.String;
                        textIterator.MoveNext(); goto case State.String;
                    }
                    else if (IsStartIdentificatorNameChar())
                    {
                        _lineIndex = textIterator.LineIndex;
                        _columnIndex = textIterator.ColumnIndex;
                        _nameLength = 0;
                        AddLexemChar();
                        textIterator.MoveNext();
                        goto case State.Identificator;
                    }
                    else if (IsDigit())
                    {
                        _lineIndex = textIterator.LineIndex;
                        _columnIndex = textIterator.ColumnIndex;
                        _nameLength = 0;
                        AddLexemChar();
                        textIterator.MoveNext();
                        goto case State.Number;
                    }
                    else if (textIterator.Char == '/' && textIterator.NextChar == '/')
                    {
                        _lineIndex = textIterator.LineIndex;
                        _columnIndex = textIterator.ColumnIndex;
                        _nameLength = 0;
                        _kind = LexemKind.Comment;
                        goto case State.Comment;
                    }
                    else if (IsOperator())
                    {
                        _lineIndex = textIterator.LineIndex;
                        _columnIndex = textIterator.ColumnIndex;
                        _kind = LexemKind.Operator;
                        MakeLexem();
                        textIterator.MoveNext();
                        goto case State.Begin;
                    }
                    else if (_textIterator!.Char == '.')
                    {
                        _lineIndex = textIterator.LineIndex;
                        _columnIndex = textIterator.ColumnIndex;
                        _kind = LexemKind.Dot;
                        MakeLexem();
                        textIterator.MoveNext();
                        goto case State.Begin;
                    }
                    else goto case State.Error;
                case State.Identificator:
                    if (textIterator.Eof) { MakeLexem(); goto case State.End; }
                    else if (IsSpace()) { MakeLexem(); textIterator.MoveNext(); goto case State.Begin; }
                    else if (IsReturn()) { MakeLexem(); textIterator.MoveNext(); goto case State.Begin; }
                    else if (textIterator.Char == ')') { MakeLexem(); goto case State.Begin; }
                    else if (IsDelimiter()) { MakeLexem(); goto case State.Begin; }
                    else if (IsIdentificatorNameChar()) { AddLexemChar(); textIterator.MoveNext(); goto case State.Identificator; }
                    else goto case State.Error;
                case State.String:
                    if (textIterator.Eof) { MakeLexem(); goto case State.End; }
                    else if (IsReturn()) { MakeLexem(); textIterator.MoveNext(); goto case State.Begin; }
                    else if (textIterator.Char == '\'') { _nameLength++; MakeLexem(); textIterator.MoveNext(); goto case State.Begin; }
                    else { _nameLength++; textIterator.MoveNext(); goto case State.String; }
                case State.Number:
                    if (textIterator.Eof) { MakeLexem(); goto case State.End; }
                    else if (IsSpace()) { MakeLexem(); textIterator.MoveNext(); goto case State.Begin; }
                    else if (IsReturn()) { MakeLexem(); textIterator.MoveNext(); goto case State.Begin; }
                    else if (textIterator.Char == '.') { _kind = LexemKind.FloatNumber; AddLexemChar(); textIterator.MoveNext(); goto case State.FloatNumber; }
                    else if (IsDelimiter()) { MakeLexem(); goto case State.Begin; }
                    else if (textIterator.Char == ')') { MakeLexem(); goto case State.Begin; }
                    else if (IsDigit()) { AddLexemChar(); textIterator.MoveNext(); goto case State.Number; }
                    else goto case State.Error;
                case State.FloatNumber:
                    if (textIterator.Eof) { MakeLexem(); goto case State.End; }
                    else if (IsSpace()) { MakeLexem(); textIterator.MoveNext(); goto case State.Begin; }
                    else if (IsReturn()) { MakeLexem(); textIterator.MoveNext(); goto case State.Begin; }
                    else if (IsDelimiter()) { MakeLexem(); goto case State.Begin; }
                    else if (IsDigit()) { AddLexemChar(); textIterator.MoveNext(); goto case State.FloatNumber; }
                    else goto case State.Error;
                case State.Comment:
                    if (textIterator.Eof) { MakeLexem(); goto case State.End; }
                    else if (IsReturn()) { MakeLexem(); textIterator.MoveNext(); goto case State.Begin; }
                    else { _nameLength++; textIterator.MoveNext(); goto case State.Comment; }
                case State.Error:
                    break;
                case State.End:
                    break;
            }

            return _lexems;
        }

        private void AddLexemChar()
        {
            _nameArray[_nameLength++] = _textIterator!.Char;
        }

        private void MakeLexem()
        {
            _lexems.Add(new(_lineIndex, _columnIndex, _nameLength, _kind ?? GetLexemKind()));
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

        private bool IsOperator()
        {
            return _textIterator!.Char == '+' || _textIterator.Char == '-' || _textIterator.Char == '*' || _textIterator.Char == '/';
        }

        private bool IsDelimiter()
        {
            return _textIterator!.Char == '(' || _textIterator!.Char == ')' || _textIterator!.Char == '.' || IsOperator();
        }

        private bool IsSpace()
        {
            return _textIterator!.Char == ' ' || _textIterator!.Char == '\t';
        }

        private bool IsReturn()
        {
            return _textIterator!.Char == '\n';
        }

        private LexemKind GetLexemKind()
        {
            if (StringUtils.StringEquals("import", _nameArray, _nameLength)) return LexemKind.ImportDirective;
            if (StringUtils.StringEquals("const", _nameArray, _nameLength)) return LexemKind.ConstDeclare;
            if (StringUtils.StringEquals("lambda", _nameArray, _nameLength)) return LexemKind.LambdaIdentificator;
            if (StringUtils.StringEquals("run", _nameArray, _nameLength)) return LexemKind.RunFunction;
            if (IsIntegerNumber()) return LexemKind.IntegerNumber;

            return LexemKind.Identificator;
        }

        private bool IsIntegerNumber()
        {
            for (int i = 0; i < _nameLength; i++) if (!Char.IsDigit(_nameArray[i])) return false;
            return true;
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
            End,
        }
    }
}
