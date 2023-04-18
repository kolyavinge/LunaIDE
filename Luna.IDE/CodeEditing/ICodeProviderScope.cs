namespace Luna.IDE.CodeEditing;

internal interface ICodeProviderScope
{
    bool IsConstant(string name);

    bool IsFunction(string tokenName);
}
