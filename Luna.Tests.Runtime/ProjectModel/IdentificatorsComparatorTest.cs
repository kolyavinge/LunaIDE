using System.Linq;
using Luna.CodeElements;
using Luna.ProjectModel;
using NUnit.Framework;

namespace Luna.Tests.ProjectModel;

internal class IdentificatorsComparatorTest
{
    [Test]
    public void Empty()
    {
        var oldIdentificators = new CodeModelScopeIdentificators(
            new[] { new ConstantDeclaration("oldConst", new IntegerValueElement(1)) },
            new[] { new FunctionDeclaration("oldFunc", Enumerable.Empty<FunctionArgument>(), new()) },
            new[] { new ConstantDeclaration("oldImportedConst", new IntegerValueElement(1)) },
            new[] { new FunctionDeclaration("oldImportedFunc", Enumerable.Empty<FunctionArgument>(), new()) });

        var newIdentificators = new CodeModelScopeIdentificators(
            new[] { new ConstantDeclaration("newConst", new IntegerValueElement(1)) },
            new[] { new FunctionDeclaration("newFunc", Enumerable.Empty<FunctionArgument>(), new()) },
            new[] { new ConstantDeclaration("newImportedConst", new IntegerValueElement(1)) },
            new[] { new FunctionDeclaration("newImportedFunc", Enumerable.Empty<FunctionArgument>(), new()) });

        var diff = IdentificatorsComparator.GetDifferent(oldIdentificators, newIdentificators);

        Assert.True(diff.RemovedDeclaredConstants.Contains("oldConst"));
        Assert.True(diff.RemovedDeclaredFunctions.Contains("oldFunc"));
        Assert.True(diff.RemovedImportedConstants.Contains("oldImportedConst"));
        Assert.True(diff.RemovedImportedFunctions.Contains("oldImportedFunc"));

        Assert.True(diff.AddedDeclaredConstants.Contains("newConst"));
        Assert.True(diff.AddedDeclaredFunctions.Contains("newFunc"));
        Assert.True(diff.AddedImportedConstants.Contains("newImportedConst"));
        Assert.True(diff.AddedImportedFunctions.Contains("newImportedFunc"));
    }
}
