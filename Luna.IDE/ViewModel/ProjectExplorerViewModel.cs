using System;
using System.Collections.Generic;
using System.Text;
using DependencyInjection;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.ViewModel
{
    public class ProjectExplorerViewModel : NotificationObject
    {
        [Inject]
        public ProjectTreeViewModel? ProjectTreeViewModel { get; set; }
    }
}
