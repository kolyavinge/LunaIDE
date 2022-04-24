using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.ViewModel
{
    public class EnvironmentWindowsViewModel : NotificationObject
    {
        public IEnvironmentWindowsManager WindowsManager { get; }

        public ICommand CloseWindowCommand { get; }

        public EnvironmentWindowsViewModel(IEnvironmentWindowsManager windowsManager)
        {
            WindowsManager = windowsManager;
            CloseWindowCommand = new ActionCommand(CloseWindow);
        }

        private void CloseWindow(object parameter)
        {
            var window = (EnvironmentWindow)parameter;
            WindowsManager.CloseWindow(window);
        }
    }
}
