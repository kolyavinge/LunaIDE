using Luna.IDE.App.Commands;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.Common;

namespace Luna.IDE.App.ViewModel;

public class MainViewModel : NotificationObject
{
    [Inject]
    public IOpenProjectCommand? OpenProjectCommand { get; set; }

    [Inject]
    public IRunProgramCommand? RunProgramCommand { get; set; }

    [Inject]
    public IMainWindowLoadedCommand? LoadedCommand { get; set; }

    [Inject]
    public IMainWindowClosedCommand? ClosedCommand { get; set; }
}
