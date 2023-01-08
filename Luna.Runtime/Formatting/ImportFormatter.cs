using System.Collections.Generic;
using Luna.Parsing;

namespace Luna.Formatting;

internal interface IImportFormatter : IFormatter
{
    void Init();
}

internal class ImportFormatter : IImportFormatter
{
    private readonly HashSet<string> _importedFiles = new();

    public void Init()
    {
        _importedFiles.Clear();
    }

    public void Format(TokenIterator iter, TokenAppender appender, ref int skippedLinesCount)
    {
        var importToken = iter.Token;
        iter.MoveNext();
        var importFileToken = iter.Token;
        if (importToken.LineIndex != importFileToken.LineIndex)
        {
            appender.Append(importToken, 0, skippedLinesCount);
            return;
        }
        iter.MoveNext();
        if (importFileToken.Kind == TokenKind.String
            && (iter.Eof || importToken.LineIndex == iter.Token.LineIndex && iter.Token.Kind == TokenKind.Comment || importToken.LineIndex != iter.Token.LineIndex))
        {
            if (!_importedFiles.Contains(importFileToken.Name))
            {
                appender.Append(importToken, 0, skippedLinesCount);
                appender.Append(importFileToken, 7, skippedLinesCount);
                _importedFiles.Add(importFileToken.Name);
                int skippedColumnsCount = importToken.StartColumnIndex + importFileToken.StartColumnIndex - (importToken.StartColumnIndex + importToken.Length) - 1;
                appender.AppendRecentTokensInLine(iter, importToken.LineIndex, skippedLinesCount, skippedColumnsCount);
            }
            else
            {
                skippedLinesCount++;
            }
        }
        else
        {
            appender.Append(importToken, 0, skippedLinesCount);
            appender.Append(importFileToken, importFileToken.StartColumnIndex - importToken.StartColumnIndex, skippedLinesCount);
            appender.AppendRecentTokensInLine(iter, importToken.LineIndex, skippedLinesCount, importToken.StartColumnIndex);
        }
    }
}
