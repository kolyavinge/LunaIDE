using System.Windows.Input;
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
        get => _autoCompleteViewModel ?? throw new NotInitializedException(nameof(AutoCompleteViewModel));
        set
        {
            _autoCompleteViewModel = value;
            _autoCompleteViewModel.Model.Completed += (s, e) => _codeTextBox?.Focus();
            _autoCompleteViewModel.Model.Init(new AutoCompleteDataContext(Model));
        }
    }

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
        }
    }

    public double HorizontalScrollBarValue
    {
        get => _horizontalScrollBarValue;
        set
        {
            _horizontalScrollBarValue = value;
            RaisePropertyChanged(() => HorizontalScrollBarValue);
        }
    }

    public CodeFileEditorViewModel(ICodeFileEditor codeFileEditor)
    {
        Model = codeFileEditor;
        CodeTextBoxLoadedCommand = new ActionCommand<IControl>(control => _codeTextBox = control);
        ShowAutoCompleteCommand = new ActionCommand(ShowAutoComplete);
        HideAutoCompleteCommand = new ActionCommand(() => AutoCompleteViewModel.Model.IsVisible = false);
        KeyDownCommand = new ActionCommand<KeyEventArgs>(KeyDown);
    }

    private void ShowAutoComplete()
    {
        if (_codeTextBox == null) throw new NotInitializedException(nameof(_codeTextBox));
        AutoCompleteViewModel.Model.IsVisible = true;
    }

    private void KeyDown(KeyEventArgs e)
    {
        var key = e.Key == Key.System ? e.SystemKey : e.Key;
        var controlPressed = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        var altPressed = (e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
        var shiftPressed = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
        var isAutoCompleteVisible = AutoCompleteViewModel.Model.IsVisible;
        // with modifiers
        if (!isAutoCompleteVisible && controlPressed && !altPressed && !shiftPressed && key == Key.Space)
        {
            ShowAutoComplete();
            e.Handled = true;
        }
        else if (!isAutoCompleteVisible && controlPressed && !altPressed && !shiftPressed && key == Key.L)
        {
            Model.DeleteSelectedLines();
            e.Handled = true;
        }
        else if (!isAutoCompleteVisible && controlPressed && !altPressed && !shiftPressed && key == Key.Z)
        {
            Model.Undo();
            e.Handled = true;
        }
        else if (!isAutoCompleteVisible && controlPressed && !altPressed && !shiftPressed && key == Key.Y)
        {
            Model.Redo();
            e.Handled = true;
        }
        else if (!isAutoCompleteVisible && controlPressed && !altPressed && shiftPressed && key == Key.U)
        {
            Model.ToUpperCase();
            e.Handled = true;
        }
        else if (!isAutoCompleteVisible && controlPressed && !altPressed && !shiftPressed && key == Key.U)
        {
            Model.ToLowerCase();
            e.Handled = true;
        }
        else if (!isAutoCompleteVisible && !controlPressed && altPressed && !shiftPressed && key == Key.Up)
        {
            Model.MoveSelectedLinesUp();
            e.Handled = true;
        }
        else if (!isAutoCompleteVisible && !controlPressed && altPressed && !shiftPressed && key == Key.Down)
        {
            Model.MoveSelectedLinesDown();
            e.Handled = true;
        }
        // without any modifiers
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.Escape)
        {
            AutoCompleteViewModel.Model.IsVisible = false;
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.Up)
        {
            AutoCompleteViewModel.Model.MoveSelectionUp();
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.Down)
        {
            AutoCompleteViewModel.Model.MoveSelectionDown();
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.PageUp)
        {
            AutoCompleteViewModel.Model.MoveSelectionPageUp(10);
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.PageDown)
        {
            AutoCompleteViewModel.Model.MoveSelectionPageDown(10);
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.Return)
        {
            AutoCompleteViewModel.Model.Complete();
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.Space)
        {
            AutoCompleteViewModel.Model.Complete();
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && (key == Key.Left || key == Key.Right))
        {
            AutoCompleteViewModel.Model.IsVisible = false;
        }
    }
}
