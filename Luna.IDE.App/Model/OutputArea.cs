using System.Collections.Generic;
using System.Linq;
using CodeHighlighter.CodeProvidering;
using CodeHighlighter.Model;
using Luna.IDE.App.Utils;
using Luna.Output;
using Luna.Runtime;
using Token = CodeHighlighter.CodeProvidering.Token;

namespace Luna.IDE.App.Model;

public interface IOutputArea : IRuntimeOutput, ICodeProvider
{
    CodeTextBoxModel CodeTextBoxModel { get; }
    void Clear();
}

public class OutputArea : IOutputArea
{
    private int _currentLine;
    private readonly List<Token> _tokens = new();

    public CodeTextBoxModel CodeTextBoxModel { get; }

    public OutputArea()
    {
        CodeTextBoxModel = new CodeTextBoxModel(this, new() { IsReadOnly = true });
        RuntimeEnvironment.StandartOutput = this;
    }

    public void Clear()
    {
        CodeTextBoxModel.IsReadOnly = false;
        _currentLine = 0;
        CodeTextBoxModel.SetText("");
        _tokens.Clear();
        CodeTextBoxModel.IsReadOnly = true;
    }

    public void SendMessage(OutputMessage message)
    {
        CodeTextBoxModel.IsReadOnly = false;
        _tokens.AddRange(message.Items.Select(item => new Token(item.Text, _currentLine, item.ColumnIndex, item.Text.Length, (byte)item.Kind)).ToList());
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
            new ((byte)OutputMessageKind.Info, ColorUtils.FromHex("5ae65c")),
            new ((byte)OutputMessageKind.Warning, ColorUtils.FromHex("d9d177")),
            new ((byte)OutputMessageKind.Error, ColorUtils.FromHex("f44753"))
        };
    }
}
