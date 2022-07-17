using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CodeHighlighter.Model;
using Luna.Functions;
using Luna.IDE.Mvvm;
using Luna.IDE.Utils;
using Luna.IDE.View;
using Luna.Parsing;
using Luna.ProjectModel;

namespace Luna.IDE.Model;

public class AutoComplete : NotificationObject
{
    private readonly KeywordsCollection _keywords;
    private readonly EmbeddedFunctionDeclarationsCollection _embeddedFunctions;
    private readonly CodeModelScope _scope;
    private ICodeFileEditor? _codeFileEditor;
    private IControl? _codeTextBox;
    private CodeTextBoxModel? _codeTextBoxModel;
    private IReadOnlyCollection<IAutoCompleteItem> _originalItems = new IAutoCompleteItem[0];
    private IReadOnlyCollection<IAutoCompleteItem> _items = new IAutoCompleteItem[0];
    private IAutoCompleteItem? _selectedItem;
    private int _selectedIndex;
    private bool _isVisible;

    public ICodeFileEditor CodeFileEditor
    {
        get => _codeFileEditor ?? throw new NotInitializedException(nameof(CodeFileEditor));
        set => _codeFileEditor = value;
    }

    public CodeTextBoxModel CodeTextBoxModel
    {
        get => _codeTextBoxModel ?? throw new NotInitializedException(nameof(CodeTextBoxModel));
        set
        {
            _codeTextBoxModel = value;
            _codeTextBoxModel.TextChanged += OnTextChanged;
        }
    }

    public ICommand CodeTextBoxLoadedCommand { get; }

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

    public AutoComplete()
    {
        _keywords = new KeywordsCollection();
        _embeddedFunctions = new EmbeddedFunctionDeclarationsCollection();
        _scope = new CodeModelScope();
        CodeTextBoxLoadedCommand = new ActionCommand<IControl>(control => _codeTextBox = control);
    }

    private void UpdateItems()
    {
        var scopeIdentificators = _scope.GetScopeIdentificators(CodeFileEditor.ProjectItem.CodeModel);
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
        var cursor = CodeTextBoxModel.TextCursor;
        var cursorToken = CodeTextBoxModel.Tokens.GetTokenOnPosition(cursor.LineIndex, cursor.ColumnIndex);
        if (cursorToken != null)
        {
            var filterText = cursorToken.Name.Substring(0, cursor.ColumnIndex - cursorToken.StartColumnIndex);
            var filteredItems = _originalItems.Where(x => x.Name.Contains(filterText)).ToList();
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
        if (_codeTextBox == null) throw new NotInitializedException(nameof(_codeTextBox));
        var cursor = CodeTextBoxModel.TextCursor;
        var cursorToken = CodeTextBoxModel.Tokens.GetTokenOnPosition(cursor.LineIndex, cursor.ColumnIndex);
        if (cursorToken != null)
        {
            CodeFileEditor.ReplaceText(cursor.LineIndex, cursorToken.StartColumnIndex, cursor.LineIndex, cursor.ColumnIndex, SelectedItem.Name);
        }
        else
        {
            CodeFileEditor.ReplaceText(cursor.LineIndex, cursor.ColumnIndex, cursor.LineIndex, cursor.ColumnIndex, SelectedItem.Name);
        }
        _codeTextBox.Focus();
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
