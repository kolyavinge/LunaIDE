using System.Windows.Input;

namespace Luna.IDE.App.Mvvm;

public abstract class Command : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public virtual bool CanExecute(object? parameter)
    {
        return true;
    }

    public abstract void Execute(object? parameter);
}

public abstract class Command<TParameter> : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public virtual bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        Execute((TParameter)parameter!);
    }

    protected abstract void Execute(TParameter parameter);
}
