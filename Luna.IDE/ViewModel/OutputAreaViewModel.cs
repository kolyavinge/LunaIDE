using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.ViewModel;

public class OutputAreaViewModel : NotificationObject
{
    public IOutputArea OutputArea { get; }

    public OutputAreaViewModel(IOutputArea outputArea)
    {
        OutputArea = outputArea;
    }
}
