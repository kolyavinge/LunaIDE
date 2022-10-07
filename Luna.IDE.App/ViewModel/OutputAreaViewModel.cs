using Luna.IDE.Common;
using Luna.IDE.Outputing;

namespace Luna.IDE.App.ViewModel;

public class OutputAreaViewModel : NotificationObject
{
    public IOutputArea OutputArea { get; }

    public OutputAreaViewModel(IOutputArea outputArea)
    {
        OutputArea = outputArea;
    }
}
