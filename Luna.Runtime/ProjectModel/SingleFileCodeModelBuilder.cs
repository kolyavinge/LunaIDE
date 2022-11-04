using System.Linq;
using Luna.Parsing;

namespace Luna.ProjectModel;

public interface ISingleFileCodeModelBuilder
{
    CodeModel BuildFor(string text);
}

public class SingleFileCodeModelBuilder : ISingleFileCodeModelBuilder
{
    public CodeModel BuildFor(string text)
    {
        var codeModel = new CodeModel();
        var scanner = new Scanner();
        var tokens = scanner.GetTokens(new TextIterator(new Text(text))).ToList();
        var iter = new TokenIterator(tokens);
        var importDirectiveParser = new ImportDirectiveParser(iter, codeModel, new ImportDirectiveParserScope(new CodeFileProjectItem[0]));
        var functionParser = new FunctionParser(iter, codeModel, new FunctionParserScope(new[] { codeModel }, codeModel));
        importDirectiveParser.Parse(); // skip imports
        functionParser.Parse();

        return codeModel;
    }
}
