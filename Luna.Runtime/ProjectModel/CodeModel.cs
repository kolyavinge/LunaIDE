using System.Collections.Generic;

namespace Luna.ProjectModel
{
    public class CodeModel
    {
        private List<ImportDirective> _imports = new();
        private ConstDeclarationDictionary _constants = new();
        private FunctionDeclarationDictionary _functions = new();

        public IReadOnlyCollection<ImportDirective> Imports => _imports;
        public IConstDeclarationDictionary Constants => _constants;
        public IFunctionDeclarationDictionary Functions => _functions;
        public Function? RunFunction { get; internal set; }

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
        public Value Value { get; }

        public ConstDeclaration(string name, Value value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
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

    public class FunctionBody : List<Value> { }

    public class FunctionDeclaration : CodeElement
    {
        public string Name { get; }
        public List<FunctionArgument> Arguments { get; }
        public FunctionBody Body { get; }

        public FunctionDeclaration(string name, List<FunctionArgument> arguments, FunctionBody body, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
        {
            Name = name;
            Arguments = arguments;
            Body = body;
        }
    }

    public abstract class Value : CodeElement
    {
        public Value(int lineIndex, int columnIndex) : base(lineIndex, columnIndex) { }
    }

    public class IntegerValue : Value
    {
        public int Value { get; }

        public IntegerValue(int value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
        {
            Value = value;
        }
    }

    public class FloatValue : Value
    {
        public double Value { get; }

        public FloatValue(double value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
        {
            Value = value;
        }
    }

    public class StringValue : Value
    {
        public string Value { get; }

        public StringValue(string value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
        {
            Value = value;
        }
    }

    public class ListValue : Value
    {
        public List<Value> Value { get; }

        public ListValue(List<Value> value, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
        {
            Value = value;
        }
    }

    public class NamedConstant : Value
    {
        public string Name { get; }

        public NamedConstant(string name, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
        {
            Name = name;
        }
    }

    public class Variable : Value
    {
        public string Name { get; }

        public Variable(string name, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
        {
            Name = name;
        }
    }

    public class Function : Value
    {
        public string Name { get; }
        public List<Value> ArgumentValues { get; }

        public Function(string name, List<Value> argumentValues, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
        {
            Name = name;
            ArgumentValues = argumentValues;
        }
    }

    public class Lambda : Value
    {
        public List<FunctionArgument> Arguments { get; }
        public List<Value> ArgumentValues { get; } = new();
        public FunctionBody Body { get; }

        public Lambda(List<FunctionArgument> arguments, FunctionBody body, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
        {
            Arguments = arguments;
            Body = body;
        }
    }
}
