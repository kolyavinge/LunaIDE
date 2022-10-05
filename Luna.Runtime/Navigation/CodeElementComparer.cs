﻿using System.Collections.Generic;
using Luna.ProjectModel;

namespace Luna.Navigation;

internal class CodeElementComparer : IComparer<CodeElement>
{
    public int Compare(CodeElement? x, CodeElement? y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        var result = x.LineIndex.CompareTo(y.LineIndex);
        if (result == 0)
        {
            result = x.ColumnIndex.CompareTo(y.ColumnIndex);
        }

        return result;
    }
}

internal class ReverseCodeElementComparer : IComparer<CodeElement>
{
    private readonly IComparer<CodeElement> _comparer;

    public ReverseCodeElementComparer(IComparer<CodeElement> comparer)
    {
        _comparer = comparer;
    }

    public int Compare(CodeElement? x, CodeElement? y)
    {
        return -_comparer.Compare(x, y);
    }
}