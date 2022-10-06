namespace Luna.IDE.CodeEditor;

class TextIteratorWrapper : Parsing.ITextIterator
{
    private readonly CodeHighlighter.CodeProvidering.ITextIterator _textIterator;

    public char Char => _textIterator.Char;
    public char NextChar => _textIterator.NextChar;
    public int LineIndex => _textIterator.LineIndex;
    public int ColumnIndex => _textIterator.ColumnIndex;
    public bool Eof => _textIterator.Eof;
    public void MoveNext() => _textIterator.MoveNext();

    public TextIteratorWrapper(CodeHighlighter.CodeProvidering.ITextIterator textIterator)
    {
        _textIterator = textIterator;
    }
}
