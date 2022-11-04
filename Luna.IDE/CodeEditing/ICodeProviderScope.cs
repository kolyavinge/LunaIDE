namespace Luna.IDE.CodeEditing;
public interface ICodeProviderScope
{
    bool IsConstant(string name);

    bool IsFunction(string tokenName);
}
