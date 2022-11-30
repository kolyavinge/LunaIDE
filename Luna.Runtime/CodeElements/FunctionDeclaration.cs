using System.Collections.Generic;
using Luna.Collections;
using Luna.ProjectModel;

namespace Luna.CodeElements;

public class FunctionDeclaration : DeclarationCodeElement, IEquatable<FunctionDeclaration?>
{
    public CodeModel CodeModel { get; }
    public string Name { get; }
    public ReadonlyArray<FunctionArgument> Arguments { get; }
    public FunctionBody Body { get; }

    public FunctionDeclaration(CodeModel codeModel, string name, IEnumerable<FunctionArgument> arguments, FunctionBody body, int lineIndex, int columnIndex)
        : base(lineIndex, columnIndex)
    {
        CodeModel = codeModel;
        Name = name;
        Arguments = new ReadonlyArray<FunctionArgument>(arguments);
        Body = body;
    }

    internal FunctionDeclaration(string name, IEnumerable<FunctionArgument> arguments, FunctionBody body)
        : this(new(), name, arguments, body, 0, 0) { }

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
