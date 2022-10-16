﻿using System.Collections.Generic;
using Luna.Collections;
using Luna.ProjectModel;

namespace Luna.CodeElements;

public class FunctionValueElement : ValueElement, IEquatable<FunctionValueElement?>
{
    public CodeModel CodeModel { get; }
    public string Name { get; }
    public ReadonlyArray<ValueElement> ArgumentValues { get; }

    public FunctionValueElement(CodeModel codeModel, string name, IEnumerable<ValueElement> argumentValues, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        CodeModel = codeModel;
        Name = name;
        ArgumentValues = new ReadonlyArray<ValueElement>(argumentValues);
    }

    internal FunctionValueElement(CodeModel codeModel, string name, IEnumerable<ValueElement> argumentValues) : this(codeModel, name, argumentValues, 0, 0) { }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as FunctionValueElement);
    }

    public bool Equals(FunctionValueElement? other)
    {
        return other is not null &&
               base.Equals(other) &&
               Name == other.Name &&
               ArgumentValues.Equals(other.ArgumentValues);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Name, ArgumentValues);
    }
}
