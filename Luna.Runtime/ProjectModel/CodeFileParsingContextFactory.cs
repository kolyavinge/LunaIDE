using System.Collections.Generic;
using Luna.Parsing;

namespace Luna.ProjectModel;

internal interface ICodeFileParsingContextFactory
{
    ICodeFileParsingContext MakeContext(IReadOnlyCollection<CodeFileProjectItem> allCodeFiles, CodeFileProjectItem currentCodeFile);
}

internal class CodeFileParsingContextFactory : ICodeFileParsingContextFactory
{
    public ICodeFileParsingContext MakeContext(IReadOnlyCollection<CodeFileProjectItem> allCodeFiles, CodeFileProjectItem currentCodeFile)
        => new CodeFileParsingContext(allCodeFiles, currentCodeFile);
}
