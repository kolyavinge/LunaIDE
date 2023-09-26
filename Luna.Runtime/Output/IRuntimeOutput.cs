using System.Collections.Generic;
using System.Linq;
using Luna.ProjectModel;

namespace Luna.Output;

public interface IRuntimeOutput
{
    void SendMessage(OutputMessage message);
}

public class OutputMessage : IEquatable<OutputMessage?>
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

    public override bool Equals(object? obj)
    {
        return Equals(obj as OutputMessage);
    }

    public bool Equals(OutputMessage? other)
    {
        return other is not null &&
               _items.SequenceEqual(other._items) &&
               (ProjectItem is null && other.ProjectItem is null
                || (ProjectItem?.Equals(other.ProjectItem) ?? false));
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var item in _items) hashCode.Add(item.GetHashCode());
        hashCode.Add(ProjectItem?.GetHashCode());

        return hashCode.ToHashCode();
    }
}

public class OutputMessageItem : IEquatable<OutputMessageItem?>
{
    public string Text { get; }
    public OutputMessageKind Kind { get; }
    public int ColumnIndex { get; internal set; }

    public OutputMessageItem(string text, OutputMessageKind kind)
    {
        Text = text;
        Kind = kind;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as OutputMessageItem);
    }

    public bool Equals(OutputMessageItem? other)
    {
        return other is not null &&
               Text == other.Text &&
               Kind == other.Kind &&
               ColumnIndex == other.ColumnIndex;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Text, Kind, ColumnIndex);
    }
}

public enum OutputMessageKind
{
    Text,
    Info,
    Warning,
    Error
}
