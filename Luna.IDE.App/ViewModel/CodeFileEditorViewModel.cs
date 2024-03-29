﻿using System.Windows.Input;
using Luna.IDE.App.Commands.CodeFileEditor;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.Mvvm;
using Luna.IDE.AutoCompletion;
using Luna.IDE.CodeEditing;
using Luna.IDE.Common;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;

namespace Luna.IDE.App.ViewModel;

[EditorFor(typeof(CodeFileProjectItem))]
public class CodeFileEditorViewModel : NotificationObject
{
    public ICodeFileEditor Model { get; set; }

    public IAutoComplete AutoComplete { get; }

    public ICommand ShowAutoCompleteCommand { get; }

    public ICommand HideAutoCompleteCommand { get; }

    public ICommand KeyDownCommand { get; }

    [Inject]
    public IGotoDeclarationCommand? GotoDeclarationCommand { get; set; }

    public CodeFileEditorViewModel(
        ICodeFileEditor codeFileEditor,
        IAutoComplete autoComplete,
        IAutoCompleteDataContextFactory autoCompleteDataContextFactory)
    {
        Model = codeFileEditor;
        AutoComplete = autoComplete;
        AutoComplete.Completed += (s, e) => Model.Focus();
        AutoComplete.Init(autoCompleteDataContextFactory.Make(Model));
        ShowAutoCompleteCommand = new ActionCommand(() => AutoComplete.IsVisible = true);
        HideAutoCompleteCommand = new ActionCommand(() => AutoComplete.IsVisible = false);
        KeyDownCommand = new ActionCommand<KeyEventArgs>(KeyDown);
    }

    private void KeyDown(KeyEventArgs e)
    {
        var key = e.Key == Key.System ? e.SystemKey : e.Key;
        var controlPressed = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        var altPressed = (e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
        var shiftPressed = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
        var isAutoCompleteVisible = AutoComplete.IsVisible;
        // with modifiers
        if (!isAutoCompleteVisible && controlPressed && !altPressed && !shiftPressed && key == Key.Space)
        {
            AutoComplete.IsVisible = true;
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
        else if (!isAutoCompleteVisible && controlPressed && !altPressed && !shiftPressed && key == Key.F)
        {
            Model.ActivateSearchPattern();
            e.Handled = true;
        }
        // without any modifiers
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.Escape)
        {
            AutoComplete.IsVisible = false;
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.Up)
        {
            AutoComplete.MoveSelectionUp();
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.Down)
        {
            AutoComplete.MoveSelectionDown();
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.PageUp)
        {
            AutoComplete.MoveSelectionPageUp(10);
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.PageDown)
        {
            AutoComplete.MoveSelectionPageDown(10);
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.Return)
        {
            AutoComplete.Complete();
            e.Handled = true;
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && key == Key.Space)
        {
            AutoComplete.Complete();
        }
        else if (isAutoCompleteVisible && !controlPressed && !altPressed && !shiftPressed && (key == Key.Left || key == Key.Right))
        {
            AutoComplete.IsVisible = false;
        }
    }
}
