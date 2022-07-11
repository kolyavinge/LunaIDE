using System.Collections.Generic;
using System.Linq;
using Luna.Functions;
using Luna.IDE.Mvvm;
using Luna.IDE.Utils;
using Luna.Parsing;
using Luna.ProjectModel;

namespace Luna.IDE.Model;

public class AutoComplete : NotificationObject
{
    private readonly KeywordsCollection _keywords;
    private readonly EmbeddedFunctionDeclarationsCollection _embeddedFunctions;
    private readonly CodeModelScope _scope;
    private CodeFileProjectItem? _codeFile;
    private IReadOnlyCollection<IAutoCompleteItem> _items = new IAutoCompleteItem[0];
    private IAutoCompleteItem? _selectedItem;
    private int _selectedIndex;

    public CodeFileProjectItem CodeFile
    {
        get => _codeFile ?? throw new HasNotInitializedYetException(nameof(CodeFile));
        set => _codeFile = value;
    }

    public IReadOnlyCollection<IAutoCompleteItem> Items
    {
        get => _items;
        set { _items = value; RaisePropertyChanged(() => Items); }
    }

    public IAutoCompleteItem? SelectedItem
    {
        get => _selectedItem;
        set { _selectedItem = value; RaisePropertyChanged(() => SelectedItem!); }
    }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set { _selectedIndex = value; RaisePropertyChanged(() => SelectedIndex); }
    }

    public AutoComplete()
    {
        _keywords = new KeywordsCollection();
        _embeddedFunctions = new EmbeddedFunctionDeclarationsCollection();
        _scope = new CodeModelScope();
    }

    public void UpdateItems()
    {
        var scopeIdentificators = _scope.GetScopeIdentificators(CodeFile.CodeModel);
        var items = new List<IAutoCompleteItem>();
        items.AddRange(_keywords.Select(x => new KeywordAutoCompleteItem(x)));
        items.AddRange(scopeIdentificators.DeclaredConstants.Select(x => new CodeElementAutoCompleteItem(x)));
        items.AddRange(scopeIdentificators.ImportedConstants.Select(x => new CodeElementAutoCompleteItem(x)));
        items.AddRange(scopeIdentificators.DeclaredFunctions.Select(x => new CodeElementAutoCompleteItem(x)));
        items.AddRange(scopeIdentificators.ImportedFunctions.Select(x => new CodeElementAutoCompleteItem(x)));
        items.AddRange(_embeddedFunctions.Select(x => new EmbeddedFunctionAutoCompleteItem(x)));
        items.Sort();
        Items = items;
        SelectedItem = null;
    }

    public void MoveSelectionUp()
    {
        if (SelectedIndex > 0) SelectedIndex--;
        else SelectedIndex = Items.Count - 1;
    }

    public void MoveSelectionDown()
    {
        if (SelectedIndex < Items.Count - 1) SelectedIndex++;
        else SelectedIndex = 0;
    }

    public void MoveSelectionPageUp(int pageSize)
    {
        SelectedIndex -= pageSize;
        if (SelectedIndex < 0) SelectedIndex = 0;
    }

    public void MoveSelectionPageDown(int pageSize)
    {
        SelectedIndex += pageSize;
        if (SelectedIndex > Items.Count - 1) SelectedIndex = Items.Count - 1;
    }
}
