using System;
using System.Collections.Generic;
using System.Text;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.ViewModel
{
    public class OutputConsoleViewModel : NotificationObject
    {
        public IOutputConsole OutputConsole { get; }

        public OutputConsoleViewModel(IOutputConsole outputConsole)
        {
            OutputConsole = outputConsole;
        }
    }
}
