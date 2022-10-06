using Luna.IDE.App.Model;
using Luna.IDE.App.Mvvm;

namespace Luna.IDE.App.ViewModel;

public class OutputAreaViewModel : NotificationObject
{
    public IOutputArea OutputArea { get; }

    public OutputAreaViewModel(IOutputArea outputArea)
    {
        OutputArea = outputArea;
    }
}
