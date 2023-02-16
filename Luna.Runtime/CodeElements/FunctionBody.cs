using System.Collections.Generic;
using System.Linq;

namespace Luna.CodeElements;

public class FunctionBody : List<ValueElement>, IEquatable<FunctionBody?>
{
    public int StartLineIndex { get; }
    public int StartColumnIndex { get; }
    public int EndLineIndex { get; }
    public int EndColumnIndex { get; }

    public FunctionBody(int startLineIndex, int startColumnIndex, int endLineIndex, int endColumnIndex, IEnumerable<ValueElement> items)
    {
        StartLineIndex = startLineIndex;
        StartColumnIndex = startColumnIndex;
        EndLineIndex = endLineIndex;
        EndColumnIndex = endColumnIndex;
        AddRange(items);
    }

    public FunctionBody(int startLineIndex, int startColumnIndex, int endLineIndex, int endColumnIndex)
        : this(startLineIndex, startColumnIndex, endLineIndex, endColumnIndex, Enumerable.Empty<ValueElement>())
    {
    }

    public FunctionBody() : this(0, 0, 0, 0, Enumerable.Empty<ValueElement>()) { }

    public override bool Equals(object? obj)
    {
        return Equals(obj as FunctionBody);
    }

    public bool Equals(FunctionBody? other)
    {
        return other is not null &&
               this.SequenceEqual(other);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var item in this)
        {
            hashCode.Add(item.GetHashCode());
        }

        return hashCode.ToHashCode();
    }
}
