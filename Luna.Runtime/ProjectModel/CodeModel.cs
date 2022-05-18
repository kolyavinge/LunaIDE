using System.Collections.Generic;
using Luna.Collections;

namespace Luna.ProjectModel;

public class CodeModel
{
    private List<ImportDirective> _imports = new();
    private ConstDeclarationDictionary _constants = new();
    private FunctionDeclarationDictionary _functions = new();

    public IReadOnlyCollection<ImportDirective> Imports => _imports;
    public IConstDeclarationDictionary Constants => _constants;
    public IFunctionDeclarationDictionary Functions => _functions;
    public FunctionValueElement? RunFunction { get; internal set; }

    internal void AddImportDirective(ImportDirective import)
    {
        _imports.Add(import);
    }

    internal void AddConstDeclaration(ConstDeclaration constDirective)
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
}

public class ConstDeclaration : CodeElement
{
    public string Name { get; }
    public ValueElement Value { get; }

    public ConstDeclaration(string name, ValueElement value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
        Value = value;
    }
}

public class FunctionArgument : CodeElement
{
    public string Name { get; }

    public FunctionArgument(string name, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
    }
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
}

public abstract class ValueElement : CodeElement
{
    public ValueElement(int lineIndex, int columnIndex) : base(lineIndex, columnIndex) { }
}

public class IntegerValue : ValueElement
{
    public int Value { get; }

    public IntegerValue(int value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Value = value;
    }
}

public class FloatValueElement : ValueElement
{
    public double Value { get; }

    public FloatValueElement(double value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Value = value;
    }
}

public class StringValueElement : ValueElement
{
    public string Value { get; }

    public StringValueElement(string value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Value = value;
    }
}

public class ListValueElement : ValueElement
{
    public ReadonlyArray<ValueElement> Value { get; }

    public ListValueElement(IEnumerable<ValueElement> value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Value = new ReadonlyArray<ValueElement>(value);
    }
}

public class NamedConstantValueElement : ValueElement
{
    public string Name { get; }

    public NamedConstantValueElement(string name, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
    }
}

public class VariableValueElement : ValueElement
{
    public string Name { get; }

    public VariableValueElement(string name, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
    }
}

public class LambdaValueElement : ValueElement
{
    public ReadonlyArray<FunctionArgument> Arguments { get; }
    public List<ValueElement> ArgumentValues { get; } = new();
    public FunctionBody Body { get; }

    public LambdaValueElement(IEnumerable<FunctionArgument> arguments, FunctionBody body, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Arguments = new ReadonlyArray<FunctionArgument>(arguments);
        Body = body;
    }
}

public class FunctionValueElement : ValueElement
{
    public string Name { get; }
    public ReadonlyArray<ValueElement> ArgumentValues { get; }

    public FunctionValueElement(string name, IEnumerable<ValueElement> argumentValues, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        Name = name;
        ArgumentValues = new ReadonlyArray<ValueElement>(argumentValues);
    }
}
