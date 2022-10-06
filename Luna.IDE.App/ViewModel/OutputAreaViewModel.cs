using Luna.IDE.Common;
using Luna.IDE.Model;

namespace Luna.IDE.App.ViewModel;

public class OutputAreaViewModel : NotificationObject
{
    public IOutputArea OutputArea { get; }

    public OutputAreaViewModel(IOutputArea outputArea)
    {
        OutputArea = outputArea;
    }
}
