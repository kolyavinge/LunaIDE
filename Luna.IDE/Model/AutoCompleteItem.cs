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
    public ImageSource? Image { get; }

    public string Name { get; }

    public string AdditionalInfo { get; }

    public AutoCompleteItem(ImageSource? image, string name, string additionalInfo)
    {
        Image = image;
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
      : base(null, keyword, "keyword")
    {
    }
}

public class CodeElementAutoCompleteItem : AutoCompleteItem
{
    public CodeElement CodeElement { get; }

    public CodeElementAutoCompleteItem(ConstantDeclaration constant)
       : base(ImageCollection.GetImage("const.png"), constant.Name, constant.Value.ToString() ?? "")
    {
        CodeElement = constant;
    }

    public CodeElementAutoCompleteItem(FunctionDeclaration func)
        : base(ImageCollection.GetImage("func.png"), func.Name, $"({String.Join(" ", func.Arguments.Select(x => x.Name))})")
    {
        CodeElement = func;
    }
}

public class EmbeddedFunctionAutoCompleteItem : AutoCompleteItem
{
    public EmbeddedFunctionAutoCompleteItem(EmbeddedFunctionDeclaration func)
      : base(ImageCollection.GetImage("func.png"), func.Name, $"({String.Join(" ", func.Arguments)})")
    {
    }
}
