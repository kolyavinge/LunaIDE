using System;

namespace Luna.IDE.Mvvm;

public class ActionCommand : Command
{
    private readonly Action _action;

    public ActionCommand(Action action)
    {
        _action = action;
    }

    public override void Execute(object parameter)
    {
        _action();
    }
}

public class ActionCommand<TParameter> : Command
{
    private readonly Action<TParameter> _action;

    public ActionCommand(Action<TParameter> action)
    {
        _action = action;
    }

    public override void Execute(object parameter)
    {
        _action((TParameter)parameter);
    }
}
