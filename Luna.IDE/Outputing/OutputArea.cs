using System.Collections.Generic;
using System.Linq;
using CodeHighlighter;
using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Common;
using CodeHighlighter.Model;
using Luna.Output;
using Luna.Runtime;
using Token = CodeHighlighter.CodeProvidering.Token;

namespace Luna.IDE.Outputing;

public interface IOutputArea : IRuntimeOutput, ICodeProvider
{
    void Clear();
}

internal class OutputArea : IOutputArea
{
    private int _currentLine;
    private readonly List<Token> _tokens = new();

    public ICodeTextBoxModel CodeTextBoxModel { get; }

    public OutputArea()
    {
        CodeTextBoxModel = CodeTextBoxModelFactory.MakeModel(this, new() { IsReadOnly = true });
        RuntimeEnvironment.StandartOutput = this;
    }

    public void Clear()
    {
        CodeTextBoxModel.IsReadOnly = false;
        _currentLine = 0;
        CodeTextBoxModel.Text = "";
        _tokens.Clear();
        CodeTextBoxModel.IsReadOnly = true;
    }

    public void SendMessage(OutputMessage message)
    {
        CodeTextBoxModel.IsReadOnly = false;
        _tokens.AddRange(message.Items.Select(item => new Token(item.Text, _currentLine, item.ColumnIndex, (byte)item.Kind)).ToList());
        _currentLine++;
        CodeTextBoxModel.MoveCursorTextEnd();
        CodeTextBoxModel.InsertText(message.Text + Environment.NewLine);
        CodeTextBoxModel.IsReadOnly = true;
    }

    public IEnumerable<Token> GetTokens(ITextIterator textIterator)
    {
        var lineIndexes = GetAllLineIndexes(textIterator).Distinct().ToHashSet();
        return _tokens.Where(x => lineIndexes.Contains(x.LineIndex));
    }

    private IEnumerable<int> GetAllLineIndexes(ITextIterator textIterator)
    {
        while (!textIterator.Eof)
        {
            yield return textIterator.LineIndex;
            textIterator.MoveNext();
        }
    }

    public IEnumerable<TokenColor> GetColors()
    {
        return new TokenColor[]
        {
            new ((byte)OutputMessageKind.Info, Color.FromHex("5ae65c")),
            new ((byte)OutputMessageKind.Warning, Color.FromHex("d9d177")),
            new ((byte)OutputMessageKind.Error, Color.FromHex("f44753"))
        };
    }
}
