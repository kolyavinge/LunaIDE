using System.Collections.Generic;
using System.Linq;
using Luna.ProjectModel;

namespace Luna.Parsing;

public interface IImportDirectiveParserScope
{
    CodeFileProjectItem? GetCodeFileByPath(string path);
}

public class ImportDirectiveParserScope : IImportDirectiveParserScope
{
    private readonly Dictionary<string, CodeFileProjectItem> _codeFiles;

    public ImportDirectiveParserScope(IEnumerable<CodeFileProjectItem> codeFiles)
    {
        _codeFiles = codeFiles.ToDictionary(k => k.PathFromRoot, v => v);
    }

    public CodeFileProjectItem? GetCodeFileByPath(string path)
    {
        return _codeFiles.TryGetValue(path, out var value) ? value : null;
    }
}
