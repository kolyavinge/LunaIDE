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
            AutoCompleteViewModel.CorrectByVerticalScrollBarValue(value);
        }
    }

    public double HorizontalScrollBarValue
    {
        get => _horizontalScrollBarValue;
        set
        {
            _horizontalScrollBarValue = value;
            RaisePropertyChanged(() => HorizontalScrollBarValue);
            AutoCompleteViewModel.CorrectByHorizontalScrollBarValue(value);
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
        AutoCompleteViewModel.Show(VerticalScrollBarValue, HorizontalScrollBarValue, _codeTextBox.ActualWidth, _codeTextBox.ActualHeight);
    }

    private void KeyDown(KeyEventArgs e)
    {
        var controlPressed = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        var isAutoCompleteVisible = AutoCompleteViewModel.Model.IsVisible;
        if (!isAutoCompleteVisible && controlPressed && e.Key == Key.Space)
        {
            ShowAutoComplete();
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && e.Key == Key.Up)
        {
            AutoCompleteViewModel.Model.MoveSelectionUp();
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && e.Key == Key.Down)
        {
            AutoCompleteViewModel.Model.MoveSelectionDown();
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && e.Key == Key.PageUp)
        {
            AutoCompleteViewModel.Model.MoveSelectionPageUp(10);
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && e.Key == Key.PageDown)
        {
            AutoCompleteViewModel.Model.MoveSelectionPageDown(10);
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && e.Key == Key.Return)
        {
            AutoCompleteViewModel.Model.Complete();
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && e.Key == Key.Space)
        {
            AutoCompleteViewModel.Model.Complete();
        }
        else if (isAutoCompleteVisible && (e.Key == Key.Left || e.Key == Key.Right))
        {
            AutoCompleteViewModel.Model.IsVisible = false;
        }
    }
}
