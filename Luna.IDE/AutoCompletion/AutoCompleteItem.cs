using System.Linq;
using Luna.CodeElements;
using Luna.Functions;

namespace Luna.IDE.AutoCompletion;

public interface IAutoCompleteItem : IComparable<IAutoCompleteItem>
{
    string? ImageName { get; }

    string Name { get; }

    string AdditionalInfo { get; }
}

internal class AutoCompleteItem : IAutoCompleteItem
{
    public string? ImageName { get; }

    public string Name { get; }

    public string AdditionalInfo { get; }

    public AutoCompleteItem(string? imageName, string name, string additionalInfo)
    {
        ImageName = imageName;
        Name = name;
        AdditionalInfo = additionalInfo;
    }

    public int CompareTo(IAutoCompleteItem? x)
    {
        if (x == null) return 1;
        return Name.CompareTo(x.Name);
    }
}

internal class KeywordAutoCompleteItem : AutoCompleteItem
{
    public KeywordAutoCompleteItem(string keyword)
      : base("keyword.png", keyword, "keyword")
    {
    }
}

internal class CodeElementAutoCompleteItem : AutoCompleteItem
{
    public CodeElement CodeElement { get; }

    public CodeElementAutoCompleteItem(ConstantDeclaration constant)
       : base("const.png", constant.Name, constant.Value.ToString() ?? "")
    {
        CodeElement = constant;
    }

    public CodeElementAutoCompleteItem(FunctionDeclaration func)
        : base("func.png", func.Name, $"({string.Join(" ", func.Arguments.Select(x => x.Name))})")
    {
        CodeElement = func;
    }
}

internal class EmbeddedFunctionAutoCompleteItem : AutoCompleteItem
{
    public EmbeddedFunctionAutoCompleteItem(EmbeddedFunctionDeclaration func)
      : base("func.png", func.Name, $"({string.Join(" ", func.Arguments)})")
    {
    }
}
