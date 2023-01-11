using System.Windows.Input;

namespace Luna.IDE.App.Mvvm;

public abstract class Command : ICommand
{
#pragma warning disable CS0067
    public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

    public virtual bool CanExecute(object? parameter)
    {
        return true;
    }

    public abstract void Execute(object? parameter);
}

public abstract class Command<TParameter> : Command, ICommand
{
    public override void Execute(object? parameter)
    {
        Execute((TParameter)parameter!);
    }

    protected abstract void Execute(TParameter parameter);
}
