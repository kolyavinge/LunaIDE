using System;
using System.Collections.Generic;
using System.Text;
using Luna.IDE.Commands;
using Luna.IDE.Infrastructure;
using Luna.IDE.Mvvm;

namespace Luna.IDE.ViewModel
{
    public class MainViewModel : NotificationObject
    {
        [Inject]
        public IOpenProjectCommand? OpenProjectCommand { get; set; }
    }
}
