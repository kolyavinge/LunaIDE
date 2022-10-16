using System.Collections.Generic;
using System.Linq;

namespace Luna.CodeElements;

public class FunctionBody : List<ValueElement>, IEquatable<FunctionBody?>
{
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
