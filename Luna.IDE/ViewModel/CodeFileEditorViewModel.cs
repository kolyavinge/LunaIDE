using System.Windows.Input;
using CodeHighlighter.Contracts;
using Luna.IDE.Infrastructure;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;
using Luna.IDE.Utils;
using Luna.IDE.View;
using Luna.ProjectModel;

namespace Luna.IDE.ViewModel;

[EditorFor(typeof(CodeFileProjectItem))]
public class CodeFileEditorViewModel : NotificationObject
{
    private double _verticalScrollBarValue;
    private double _horizontalScrollBarValue;
    private IControl? _codeTextBox;
    private AutoCompleteViewModel? _autoCompleteViewModel;

    public ICodeFileEditor Model { get; set; }

    [Inject]
    public AutoCompleteViewModel AutoCompleteViewModel
    {
        get => _autoCompleteViewModel ?? throw new HasNotInitializedYetException(nameof(AutoCompleteViewModel));
        set
        {
            _autoCompleteViewModel = value;
            _autoCompleteViewModel.Model.CodeFile = Model.ProjectItem;
            _autoCompleteViewModel.CompleteCommand = new ActionCommand<IAutoCompleteItem?>(AutoCompleteSelection);
        }
    }

    public ICommand CodeTextBoxModelLoadedCommand { get; set; }

    public ICommand CodeTextBoxLoadedCommand { get; }

    public ICommand ShowAutoCompleteCommand { get; }

    public ICommand HideAutoCompleteCommand { get; }

    public ICommand KeyDownCommand { get; }

    public double VerticalScrollBarValue
    {
        get => _verticalScrollBarValue;
        set
        {
            _verticalScrollBarValue = value;
            RaisePropertyChanged(() => VerticalScrollBarValue);
            if (AutoCompleteViewModel!.IsVisible) AutoCompleteViewModel.CorrectByVerticalScrollBarValue(value);
        }
    }

    public double HorizontalScrollBarValue
    {
        get => _horizontalScrollBarValue;
        set
        {
            _horizontalScrollBarValue = value;
            RaisePropertyChanged(() => HorizontalScrollBarValue);
            if (AutoCompleteViewModel!.IsVisible) AutoCompleteViewModel.CorrectByHorizontalScrollBarValue(value);
        }
    }

    public CodeFileEditorViewModel(ICodeFileEditor codeFileEditor)
    {
        Model = codeFileEditor;
        CodeTextBoxModelLoadedCommand = new ActionCommand<CodeTextBoxModel>(model => Model.CodeTextBoxModel = model);
        CodeTextBoxLoadedCommand = new ActionCommand<IControl>(control => _codeTextBox = control);
        ShowAutoCompleteCommand = new ActionCommand(ShowAutoComplete);
        HideAutoCompleteCommand = new ActionCommand(() => AutoCompleteViewModel.IsVisible = false);
        KeyDownCommand = new ActionCommand<KeyEventArgs>(KeyDown);
    }

    private void ShowAutoComplete()
    {
        if (_codeTextBox == null) throw new HasNotInitializedYetException(nameof(_codeTextBox));
        var cursorPosition = Model.CodeTextBoxModel.TextCursor;
        var textMeasures = Model.CodeTextBoxModel.TextMeasures;
        var x = (cursorPosition.ColumnIndex + 1) * textMeasures.LetterWidth;
        var y = (cursorPosition.LineIndex + 1) * textMeasures.LineHeight;
        AutoCompleteViewModel.Show(new(x, y), VerticalScrollBarValue, HorizontalScrollBarValue, _codeTextBox.ActualWidth, _codeTextBox.ActualHeight);
    }

    private void AutoCompleteSelection(IAutoCompleteItem? selectedItem)
    {
        if (selectedItem == null) return;
        if (_codeTextBox == null) throw new HasNotInitializedYetException(nameof(_codeTextBox));
        Model.InsertText(selectedItem.Name);
        _codeTextBox.Focus();
    }

    private void KeyDown(KeyEventArgs e)
    {
        var controlPressed = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        if (!AutoCompleteViewModel.IsVisible && controlPressed && e.Key == Key.Space)
        {
            ShowAutoComplete();
            e.Handled = true;
        }
        else if (AutoCompleteViewModel.IsVisible && e.Key == Key.Up)
        {
            AutoCompleteViewModel.Model.MoveSelectionUp();
            e.Handled = true;
        }
        else if (AutoCompleteViewModel.IsVisible && e.Key == Key.Down)
        {
            AutoCompleteViewModel.Model.MoveSelectionDown();
            e.Handled = true;
        }
        else if (AutoCompleteViewModel.IsVisible && e.Key == Key.PageUp)
        {
            AutoCompleteViewModel.Model.MoveSelectionPageUp(10);
            e.Handled = true;
        }
        else if (AutoCompleteViewModel.IsVisible && e.Key == Key.PageDown)
        {
            AutoCompleteViewModel.Model.MoveSelectionPageDown(10);
            e.Handled = true;
        }
        else if (AutoCompleteViewModel.IsVisible && e.Key == Key.Return)
        {
            AutoCompleteViewModel.Complete();
            e.Handled = true;
        }
        else if (AutoCompleteViewModel.IsVisible && e.Key == Key.Space)
        {
            AutoCompleteViewModel.Complete();
        }
        else if (AutoCompleteViewModel.IsVisible && (e.Key == Key.Left || e.Key == Key.Right))
        {
            AutoCompleteViewModel.IsVisible = false;
        }
    }
}
