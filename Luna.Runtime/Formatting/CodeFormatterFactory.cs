namespace Luna.Formatting;

public interface ICodeFormatterFactory
{
    ICodeFormatter Make();
}

public class CodeFormatterFactory : ICodeFormatterFactory
{
    public ICodeFormatter Make()
    {
        return new CodeFormatter(
            new CommentFormatter(),
            new ImportFormatter(),
            new ConstantFormatter(),
            new DefaultFormatter());
    }
}
