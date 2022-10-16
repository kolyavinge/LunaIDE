﻿using System.Collections.Generic;
using Luna.Collections;

namespace Luna.CodeElements;

public class FunctionDeclaration : DeclarationCodeElement, IEquatable<FunctionDeclaration?>
{
    public string Name { get; }
    public ReadonlyArray<FunctionArgument> Arguments { get; }
    public FunctionBody Body { get; }

    public FunctionDeclaration(string name, IEnumerable<FunctionArgument> arguments, FunctionBody body, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
        Arguments = new ReadonlyArray<FunctionArgument>(arguments);
        Body = body;
    }

    internal FunctionDeclaration(string name, IEnumerable<FunctionArgument> arguments, FunctionBody body) : this(name, arguments, body, 0, 0) { }

    public override bool Equals(object? obj)
    {
        return Equals(obj as FunctionDeclaration);
    }

    public bool Equals(FunctionDeclaration? other)
    {
        return other is not null &&
               base.Equals(other) &&
               Name == other.Name &&
               Arguments.Equals(other.Arguments) &&
               Body.Equals(other.Body);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Name, Arguments, Body);
    }
}
