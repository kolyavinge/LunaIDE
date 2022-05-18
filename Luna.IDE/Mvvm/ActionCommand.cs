using System;
using System.Windows.Input;

namespace Luna.IDE.Mvvm;

public class ActionCommand : ICommand
{
    private readonly Action? _action;
    private readonly Action<object>? _actionWithObject;

    public event EventHandler? CanExecuteChanged;

    public ActionCommand(Action action)
    {
        _action = action;
    }

    public ActionCommand(Action<object> action)
    {
        _actionWithObject = action;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        if (_action != null) _action();
        else if (_actionWithObject != null) _actionWithObject(parameter);
    }
}
