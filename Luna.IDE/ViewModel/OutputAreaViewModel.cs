using Luna.IDE.Mvvm;
using Luna.IDE.Model;

namespace Luna.IDE.ViewModel
{
    public class OutputAreaViewModel : NotificationObject
    {
        public IOutputArea OutputArea { get; }

        public OutputAreaViewModel(IOutputArea outputArea)
        {
            OutputArea = outputArea;
        }
    }
}
