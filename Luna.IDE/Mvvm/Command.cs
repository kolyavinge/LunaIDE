using System;
using System.Windows.Input;

namespace Luna.IDE.Mvvm
{
    public abstract class Command : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);
    }
}
