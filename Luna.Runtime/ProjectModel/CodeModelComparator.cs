using System.Collections.Generic;
using System.Linq;

namespace Luna.ProjectModel;

public class CodeModelDifferent
{
    public IReadOnlyCollection<FunctionDeclaration> RemovedFunctions { get; }

    public IReadOnlyCollection<FunctionDeclaration> AddedFunctions { get; }

    public CodeModelDifferent(IReadOnlyCollection<FunctionDeclaration> removedFunctions, IReadOnlyCollection<FunctionDeclaration> newFunctions)
    {
        RemovedFunctions = removedFunctions;
        AddedFunctions = newFunctions;
    }
}

public static class CodeModelComparator
{
    public static CodeModelDifferent GetDifferent(CodeModel oldModel, CodeModel newModel)
    {
        var removedFunctions = oldModel.Functions.Where(f => !newModel.Functions.Contains(f.Name)).ToList();
        var newFunctions = newModel.Functions.Where(f => !oldModel.Functions.Contains(f.Name)).ToList();

        return new(removedFunctions, newFunctions);
    }
}
