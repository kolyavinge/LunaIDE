using CodeHighlighter.CodeProvidering;
using Luna.IDE.CodeEditing;
using Luna.ProjectModel;

namespace Luna.IDE.TextDiff;

public interface ITextDiffCodeProviderFactory
{
    ICodeProvider Make(string fileExtension, string oldFileText, string newFileText);
}

public class TextDiffCodeProviderFactory : ITextDiffCodeProviderFactory
{
    private readonly ISingleFileCodeModelBuilder _codeModelBuilder;

    public TextDiffCodeProviderFactory(ISingleFileCodeModelBuilder codeModelBuilder)
    {
        _codeModelBuilder = codeModelBuilder;
    }

    public ICodeProvider Make(string fileExtension, string oldFileText, string newFileText)
    {
        if (fileExtension.Equals(".luna", StringComparison.OrdinalIgnoreCase))
        {
            var oldCodeModel = _codeModelBuilder.BuildFor(oldFileText);
            var newCodeModel = _codeModelBuilder.BuildFor(newFileText);

            return new LunaCodeProvider(new CompositeCodeProviderScope(new[] { oldCodeModel, newCodeModel }));
        }

        throw new ArgumentException($"CodeProvider for {fileExtension} extension isn't exist.");
    }
}
