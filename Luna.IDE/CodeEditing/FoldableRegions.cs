using System.Collections.Generic;
using System.Linq;
using CodeHighlighter.Model;
using Luna.Parsing;

namespace Luna.IDE.CodeEditing;

public readonly struct FoldableRegion
{
    public readonly int LineIndex;
    public readonly int LinesCount;

    public FoldableRegion(int lineIndex, int linesCount)
    {
        LineIndex = lineIndex;
        LinesCount = linesCount;
    }
}

public interface IFoldableRegions
{
    IEnumerable<FoldableRegion> GetRegions(ITokenCollection tokens);
}

public class FoldableRegions : IFoldableRegions
{
    private static readonly ISet<byte> _foldableTokenKinds = new HashSet<byte>
    {
        (byte)TokenKind.ImportDirective,
        (byte)TokenKind.ConstDeclaration,
        (byte)TokenKind.Comment
    };

    private readonly List<FoldableRegion> _regions = new();
    private int _lineIndex;
    private ITokenCollection? _tokens;

    public IEnumerable<FoldableRegion> GetRegions(ITokenCollection tokens)
    {
        _regions.Clear();
        _lineIndex = -1;
        _tokens = tokens;
        byte currentTokenKind = 0;
        int startLineIndex = 0;
        int endLineIndex = 0;
        var brackets = new Stack<int>();
        NextUnemptyLine();
        switch (1)
        {
            case 1:
                if (_lineIndex >= tokens.LinesCount) break;
                else
                {
                    currentTokenKind = tokens.GetTokens(_lineIndex).First().Kind;
                    if (_foldableTokenKinds.Contains(currentTokenKind)) { startLineIndex = _lineIndex; endLineIndex = _lineIndex; NextUnemptyLine(); goto case 2; }
                    else if (currentTokenKind == (byte)TokenKind.OpenBracket) { goto case 3; }
                    else { NextUnemptyLine(); goto case 1; }
                }
            case 2:
                if (_lineIndex >= tokens.LinesCount) { _regions.Add(new(startLineIndex, endLineIndex - startLineIndex)); break; }
                else
                {
                    if (currentTokenKind == tokens.GetTokens(_lineIndex).First().Kind) { endLineIndex = _lineIndex; NextUnemptyLine(); goto case 2; }
                    else { _regions.Add(new(startLineIndex, endLineIndex - startLineIndex)); goto case 1; }
                }
            case 3:
                bool end = false;
                while (_lineIndex < tokens.LinesCount && !end)
                {
                    foreach (var token in tokens.GetTokens(_lineIndex))
                    {
                        if (token.Kind == (byte)TokenKind.OpenBracket) brackets.Push(_lineIndex);
                        else if (token.Kind == (byte)TokenKind.CloseBracket)
                        {
                            startLineIndex = brackets.Pop();
                            _regions.Add(new(startLineIndex, _lineIndex - startLineIndex));
                            if (!brackets.Any()) { end = true; break; }
                        }
                    }
                    NextUnemptyLine();
                }
                goto case 1;
        }

        return _regions
            .GroupBy(x => x.LineIndex)
            .Select(g => g.OrderByDescending(x => x.LinesCount).First())
            .Where(x => x.LinesCount > 0)
            .ToList();
    }

    private void NextUnemptyLine()
    {
        _lineIndex++;
        while (_lineIndex < _tokens!.LinesCount && !_tokens.GetTokens(_lineIndex).Any()) _lineIndex++;
    }
}
