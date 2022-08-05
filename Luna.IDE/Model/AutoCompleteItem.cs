using System;
using System.Linq;
using System.Windows.Media;
using Luna.Functions;
using Luna.IDE.Media;
using Luna.ProjectModel;

namespace Luna.IDE.Model;

public interface IAutoCompleteItem : IComparable<IAutoCompleteItem>
{
    ImageSource? Image { get; }

    string Name { get; }

    string AdditionalInfo { get; }
}

public class AutoCompleteItem : IAutoCompleteItem
{
    // чтобы в тестах не грузились картинки
    private readonly Func<ImageSource?>? _imageFunc;
    public ImageSource? Image => _imageFunc?.Invoke();

    public string Name { get; }

    public string AdditionalInfo { get; }

    public AutoCompleteItem(Func<ImageSource?>? imageFunc, string name, string additionalInfo)
    {
        _imageFunc = imageFunc;
        Name = name;
        AdditionalInfo = additionalInfo;
    }

    public int CompareTo(IAutoCompleteItem? x)
    {
        if (x == null) return 1;
        return Name.CompareTo(x.Name);
    }
}

public class KeywordAutoCompleteItem : AutoCompleteItem
{
    public KeywordAutoCompleteItem(string keyword)
      : base(() => ImageCollection.GetImage("keyword.png"), keyword, "keyword")
    {
    }
}

public class CodeElementAutoCompleteItem : AutoCompleteItem
{
    public CodeElement CodeElement { get; }

    public CodeElementAutoCompleteItem(ConstantDeclaration constant)
       : base(() => ImageCollection.GetImage("const.png"), constant.Name, constant.Value.ToString() ?? "")
    {
        CodeElement = constant;
    }

    public CodeElementAutoCompleteItem(FunctionDeclaration func)
        : base(() => ImageCollection.GetImage("func.png"), func.Name, $"({String.Join(" ", func.Arguments.Select(x => x.Name))})")
    {
        CodeElement = func;
    }
}

public class EmbeddedFunctionAutoCompleteItem : AutoCompleteItem
{
    public EmbeddedFunctionAutoCompleteItem(EmbeddedFunctionDeclaration func)
      : base(() => ImageCollection.GetImage("func.png"), func.Name, $"({String.Join(" ", func.Arguments)})")
    {
    }
}
