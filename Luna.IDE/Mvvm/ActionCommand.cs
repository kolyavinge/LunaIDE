using System;

namespace Luna.IDE.Mvvm;

public class ActionCommand : Command
{
    private readonly Action? _action;
    private readonly Action<object>? _actionWithObject;

    public ActionCommand(Action action)
    {
        _action = action;
    }

    public ActionCommand(Action<object> action)
    {
        _actionWithObject = action;
    }

    public override void Execute(object parameter)
    {
        if (_action != null) _action();
        else _actionWithObject?.Invoke(parameter);
    }
}
