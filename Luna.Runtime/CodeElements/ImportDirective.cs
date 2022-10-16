using Luna.ProjectModel;

namespace Luna.CodeElements;

public class ImportDirective : CodeElement, IEquatable<ImportDirective?>
{
    public string FilePath { get; }

    public CodeFileProjectItem CodeFile { get; }

    public ImportDirective(string filePath, CodeFileProjectItem codeFile, int lineIndex, int columnIndex) : base(lineIndex, columnIndex)
    {
        FilePath = filePath;
        CodeFile = codeFile;
    }

    internal ImportDirective(string filePath, CodeFileProjectItem codeFile) : this(filePath, codeFile, 0, 0) { }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ImportDirective);
    }

    public bool Equals(ImportDirective? other)
    {
        return other is not null &&
               base.Equals(other) &&
               FilePath == other.FilePath;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), FilePath);
    }
}
