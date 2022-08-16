using System.Collections.Generic;
using System.Linq;
using Luna.ProjectModel;

namespace Luna.Output;

public interface IRuntimeOutput
{
    void SendMessage(OutputMessage message);
}

public class OutputMessage
{
    private readonly List<OutputMessageItem> _items;

    public IReadOnlyCollection<OutputMessageItem> Items => _items;

    public string Text => String.Join("", Items.Select(x => x.Text));

    public ProjectItem? ProjectItem { get; internal set; }

    public OutputMessage(IEnumerable<OutputMessageItem> items)
    {
        _items = items.ToList();
        for (int i = 1; i < _items.Count; i++)
        {
            _items[i].ColumnIndex = _items[i - 1].ColumnIndex + _items[i - 1].Text.Length;
        }
    }
}

public class OutputMessageItem
{
    public string Text { get; }
    public OutputMessageKind Kind { get; }
    public int ColumnIndex { get; internal set; }

    public OutputMessageItem(string text, OutputMessageKind kind)
    {
        Text = text;
        Kind = kind;
    }
}

public enum OutputMessageKind
{
    Text,
    Info,
    Warning,
    Error
}
