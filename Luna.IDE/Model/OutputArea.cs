﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeHighlighter;
using CodeHighlighter.Contracts;
using CodeHighlighter.Commands;
using Luna.IDE.Utils;
using Luna.Output;

namespace Luna.IDE.Model;

public interface IOutputArea : IRuntimeOutput, ICodeProvider
{
    CodeTextBoxModel CodeTextBoxModel { get; set; }
    void Clear();
}

public class OutputArea : IOutputArea
{
    private int _currentLine;
    private CodeTextBoxModel? _codeTextBoxModel;
    private readonly List<Token> _tokens = new();

    public CodeTextBoxModel CodeTextBoxModel
    {
        get => _codeTextBoxModel ?? throw new HasNotInitializedYetException(nameof(CodeTextBoxModel));
        set => _codeTextBoxModel = value;
    }

    public CodeTextBoxCommands TextCommands { get; } = new();

    public void Clear()
    {
        _currentLine = 0;
        CodeTextBoxModel.Text.TextContent = "";
        _tokens.Clear();
    }

    public void NewMessage(OutputMessage message)
    {
        _tokens.AddRange(message.Items.Select(item => new Token(item.Text, _currentLine, item.ColumnIndex, item.Text.Length, (byte)item.Kind)).ToList());
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
            new ((byte)OutputMessageKind.Info, ColorUtils.FromHex("5ae65c")),
            new ((byte)OutputMessageKind.Warning, ColorUtils.FromHex("d9d177")),
            new ((byte)OutputMessageKind.Error, ColorUtils.FromHex("f44753"))
        };
    }
}
