using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using CodeHighlighter;
using CodeHighlighter.Commands;
using Luna.Output;

namespace Luna.IDE.Model;

public interface IOutputArea : IRuntimeOutput, ICodeProvider
{
    void Clear();
}

public class OutputArea : IOutputArea
{
    private int _currentLine;
    private readonly List<Token> _tokens = new();

    public CodeTextBoxCommands TextCommands { get; } = new();
    public TextHolder TextHolder { get; } = new();

    public void Clear()
    {
        _currentLine = 0;
        TextHolder.TextValue = "";
        _tokens.Clear();
    }

    public void NewMessage(OutputMessage message)
    {
        _tokens.AddRange(message.Items.Select(item => new Token(_currentLine, item.ColumnIndex, item.Text.Length, (byte)item.Kind)).ToList());
        _currentLine++;
        TextCommands.MoveCursorTextEndCommand.Execute();
        TextCommands.InsertTextCommand.Execute(new InsertTextCommandParameter(message.Text + Environment.NewLine));
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
            new ((byte)OutputMessageKind.Info, new() { R = 78, G = 220, B = 57, A = 255 }),
            new ((byte)OutputMessageKind.Warning, Colors.Yellow),
            new ((byte)OutputMessageKind.Error, new() { R = 244, G = 71, B = 83, A = 255 }),
        };
    }
}
