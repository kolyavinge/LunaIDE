using Luna.Parsing;

namespace Luna.Formatting;

public interface ICodeFormatter
{
    string Format(string text);
}

internal class CodeFormatter : ICodeFormatter
{
    private readonly ICommentFormatter _commentFormatter;
    private readonly IImportFormatter _importFormatter;
    private readonly IConstantFormatter _constantFormatter;
    private readonly IDefaultFormatter _defaultFormatter;

    public CodeFormatter(
        ICommentFormatter commentFormatter,
        IImportFormatter importFormatter,
        IConstantFormatter constantFormatter,
        IDefaultFormatter defaultFormatter)
    {
        _commentFormatter = commentFormatter;
        _importFormatter = importFormatter;
        _constantFormatter = constantFormatter;
        _defaultFormatter = defaultFormatter;
    }

    public string Format(string text)
    {
        var scanner = new Scanner();
        var tokens = scanner.GetTokens(new TextIterator(new Text(text)));
        _importFormatter.Init();
        _constantFormatter.Init(tokens);
        var iter = new TokenIterator(tokens, new() { SkipComments = false });
        var formatted = new CodeStringBuilder(text.Length);
        var appender = new TokenAppender(formatted);
        int skippedLinesCount = 0;
        while (!iter.Eof)
        {
            if (iter.Token.Kind == TokenKind.Comment)
            {
                _commentFormatter.Format(iter, appender, ref skippedLinesCount);
            }
            else if (iter.Token.Kind == TokenKind.ImportDirective)
            {
                _importFormatter.Format(iter, appender, ref skippedLinesCount);
            }
            else if (iter.Token.Kind == TokenKind.ConstDeclaration)
            {
                _constantFormatter.Format(iter, appender, ref skippedLinesCount);
            }
            else
            {
                _defaultFormatter.Format(iter, appender, ref skippedLinesCount);
            }
        }
        formatted.AppendLine();

        return formatted.ToString();
    }
}
