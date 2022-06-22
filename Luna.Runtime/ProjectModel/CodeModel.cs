using System.Collections.Generic;
using Luna.Collections;

namespace Luna.ProjectModel;

public class CodeModel
{
    private readonly List<ImportDirective> _imports = new();
    private readonly ConstantDeclarationDictionary _constants = new();
    private readonly FunctionDeclarationDictionary _functions = new();

    public IReadOnlyCollection<ImportDirective> Imports => _imports;
    public IConstantDeclarationDictionary Constants => _constants;
    public IFunctionDeclarationDictionary Functions => _functions;
    public FunctionValueElement? RunFunction { get; internal set; }

    internal void AddImportDirective(ImportDirective import)
    {
        _imports.Add(import);
    }

    internal void AddConstantDeclaration(ConstantDeclaration constDirective)
    {
        _constants.Add(constDirective);
    }

    internal void AddFunctionDeclaration(FunctionDeclaration function)
    {
        _functions.Add(function);
    }
}

public abstract class CodeElement
{
    public int LineIndex { get; }
    public int ColumnIndex { get; }

    protected CodeElement(int lineIndex, int columnIndex)
    {
        LineIndex = lineIndex;
        ColumnIndex = columnIndex;
    }
}

public class ImportDirective : CodeElement
{
    public string FilePath { get; }

    public CodeFileProjectItem CodeFile { get; }

    public ImportDirective(string filePath, CodeFileProjectItem codeFile, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        FilePath = filePath;
        CodeFile = codeFile;
    }

    internal ImportDirective(string filePath, CodeFileProjectItem codeFile) : this(filePath, codeFile, 0, 0) { }
}

public class ConstantDeclaration : CodeElement
{
    public string Name { get; }
    public ValueElement Value { get; }

    public ConstantDeclaration(string name, ValueElement value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
        Value = value;
    }

    internal ConstantDeclaration(string name, ValueElement value) : this(name, value, 0, 0) { }
}

public class FunctionArgument : CodeElement
{
    public string Name { get; }

    public FunctionArgument(string name, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
    }

    internal FunctionArgument(string name) : this(name, 0, 0) { }
}

public class FunctionBody : List<ValueElement> { }

public class FunctionDeclaration : CodeElement
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
}

public abstract class ValueElement : CodeElement
{
    protected ValueElement(int lineIndex, int columnIndex) : base(lineIndex, columnIndex) { }
}

public class BooleanValueElement : ValueElement
{
    public bool Value { get; }

    public BooleanValueElement(bool value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Value = value;
    }

    internal BooleanValueElement(bool value) : this(value, 0, 0) { }
}

public class IntegerValueElement : ValueElement
{
    public int Value { get; }

    public IntegerValueElement(int value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Value = value;
    }

    internal IntegerValueElement(int value) : this(value, 0, 0) { }
}

public class FloatValueElement : ValueElement
{
    public double Value { get; }

    public FloatValueElement(double value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Value = value;
    }

    internal FloatValueElement(double value) : this(value, 0, 0) { }
}

public class StringValueElement : ValueElement
{
    public string Value { get; }

    public StringValueElement(string value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Value = value;
    }

    internal StringValueElement(string value) : this(value, 0, 0) { }
}

public class ListValueElement : ValueElement
{
    public ReadonlyArray<ValueElement> Items { get; }

    public ListValueElement(IEnumerable<ValueElement> items, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Items = new ReadonlyArray<ValueElement>(items);
    }

    internal ListValueElement(IEnumerable<ValueElement> items) : this(items, 0, 0) { }
}

public class NamedConstantValueElement : ValueElement
{
    public string Name { get; }

    public NamedConstantValueElement(string name, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
    }

    internal NamedConstantValueElement(string name) : this(name, 0, 0) { }
}

public class VariableValueElement : ValueElement
{
    public string Name { get; }

    public VariableValueElement(string name, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
    }

    internal VariableValueElement(string name) : this(name, 0, 0) { }
}

public class FunctionArgumentValueElement : ValueElement
{
    public string Name { get; }

    public FunctionArgumentValueElement(string name, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
    }

    internal FunctionArgumentValueElement(string name) : this(name, 0, 0) { }
}

public class LambdaValueElement : ValueElement
{
    public ReadonlyArray<FunctionArgument> Arguments { get; }
    public FunctionBody Body { get; }

    public LambdaValueElement(IEnumerable<FunctionArgument> arguments, FunctionBody body, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Arguments = new ReadonlyArray<FunctionArgument>(arguments);
        Body = body;
    }

    internal LambdaValueElement(IEnumerable<FunctionArgument> arguments, FunctionBody body) : this(arguments, body, 0, 0) { }
}

public class FunctionValueElement : ValueElement
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
}
