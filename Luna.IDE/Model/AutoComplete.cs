using System;
using System.Collections.Generic;
using System.Linq;
using Luna.Functions;
using Luna.IDE.CodeEditor;
using Luna.IDE.Mvvm;
using Luna.IDE.Utils;
using Luna.Parsing;
using Luna.ProjectModel;

namespace Luna.IDE.Model;

public class AutoComplete : NotificationObject
{
    private readonly KeywordsCollection _keywords = new();
    private readonly EmbeddedFunctionDeclarationsCollection _embeddedFunctions = new();
    private readonly CodeModelScope _scope = new();
    private IReadOnlyCollection<IAutoCompleteItem> _originalItems = new IAutoCompleteItem[0];
    private IReadOnlyCollection<IAutoCompleteItem> _items = new IAutoCompleteItem[0];
    private IAutoCompleteItem? _selectedItem;
    private IAutoCompleteDataContext? _dataContext;
    private int _selectedIndex;
    private bool _isVisible;

    public event EventHandler? Completed;

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

    public bool IsVisible
    {
        get => _isVisible;
        set { _isVisible = value; RaisePropertyChanged(() => IsVisible); }
    }

    public IAutoCompleteDataContext DataContext
    {
        get => _dataContext ?? throw new NotInitializedException(nameof(DataContext));
        set => _dataContext = value;
    }

    public void Init(IAutoCompleteDataContext dataContext)
    {
        DataContext = dataContext;
        dataContext.TextChanged += OnTextChanged;
    }

    public void Show()
    {
        UpdateItems();
        IsVisible = true;
        ProcessFilterText();
    }

    public void Complete()
    {
        IsVisible = false;
        if (SelectedItem == null) return;
        var cursor = DataContext.CursorPosition;
        var cursorToken = GetCursorToken();
        if (cursorToken != null)
        {
            DataContext.ReplaceText(new(cursor.LineIndex, cursorToken.StartColumnIndex), cursor, SelectedItem.Name);
        }
        else
        {
            DataContext.ReplaceText(cursor, cursor, SelectedItem.Name);
        }
        Completed?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateItems()
    {
        var scopeIdentificators = _scope.GetScopeIdentificators(DataContext.CodeModel);
        var items = new List<IAutoCompleteItem>();
        items.AddRange(_keywords.Select(x => new KeywordAutoCompleteItem(x)));
        items.AddRange(scopeIdentificators.DeclaredConstants.Select(x => new CodeElementAutoCompleteItem(x)));
        items.AddRange(scopeIdentificators.ImportedConstants.Select(x => new CodeElementAutoCompleteItem(x)));
        items.AddRange(scopeIdentificators.DeclaredFunctions.Select(x => new CodeElementAutoCompleteItem(x)));
        items.AddRange(scopeIdentificators.ImportedFunctions.Select(x => new CodeElementAutoCompleteItem(x)));
        items.AddRange(_embeddedFunctions.Select(x => new EmbeddedFunctionAutoCompleteItem(x)));
        items.Sort();
        _originalItems = items;
        Items = items;
        SelectedItem = null;
    }

    private void ProcessFilterText()
    {
        var cursorToken = GetCursorToken();
        if (cursorToken != null)
        {
            var cursor = DataContext.CursorPosition;
            var filterText = cursorToken.Name.Substring(0, cursor.ColumnIndex - cursorToken.StartColumnIndex);
            var filteredItems = _originalItems.Where(x => x.Name.Contains(filterText, StringComparison.OrdinalIgnoreCase)).ToList();
            if (filteredItems.Any()) Items = filteredItems;
            else Items = _originalItems;
            SelectedItem = Items.FirstOrDefault();
        }
        else
        {
            Items = _originalItems;
            SelectedItem = null;
        }
    }

    private void OnTextChanged(object? sender, EventArgs e)
    {
        if (IsVisible)
        {
            ProcessFilterText();
        }
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

    public CodeHighlighter.Model.Token? GetCursorToken()
    {
        var token = DataContext.GetTokenOnCursorPosition();
        return token != null && (token.IsIdentificator() || token.IsKeyword() || token.IsOperator()) ? token : null;
    }
}
