using CodeHighlighter.CodeProvidering;
using Luna.IDE.CodeEditing;
using Luna.ProjectModel;

namespace Luna.IDE.TextDiff;

public interface ITextDiffCodeProviderFactory
{
    ICodeProvider Make(string fileExtension, string oldFileText, string newFileText);
    ICodeProvider Make(string oldFileText, TextFileProjectItem newFile);
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

    public ICodeProvider Make(string oldFileText, TextFileProjectItem newFile)
    {
        if (newFile is CodeFileProjectItem codeFileProjectItem)
        {
            var oldCodeModel = _codeModelBuilder.BuildFor(oldFileText);

            return new LunaCodeProvider(new CompositeCodeProviderScope(new[] { oldCodeModel, codeFileProjectItem.CodeModel }));
        }

        throw new ArgumentException($"CodeProvider for {newFile.GetType()} isn't exist.");
    }
}
