using System.Text;

namespace Luna.Formatting;

internal class CodeStringBuilder
{
    private readonly StringBuilder _stringBuilder = new();
    private int _linesCount;
    private int _lastLineLength;

    public void Append(int lineIndex, int columnIndex, string text)
    {
        if (lineIndex > _linesCount)
        {
            _lastLineLength = 0;
        }

        while (_linesCount < lineIndex)
        {
            _stringBuilder.AppendLine();
            _linesCount++;
        }

        while (_lastLineLength < columnIndex)
        {
            _stringBuilder.Append(' ');
            _lastLineLength++;
        }

        _stringBuilder.Append(text);
        _lastLineLength += text.Length;
    }

    public void AppendLine()
    {
        _stringBuilder.AppendLine();
    }

    public override string ToString()
    {
        return _stringBuilder.ToString();
    }
}
