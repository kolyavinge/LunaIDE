using Luna.IDE.Common;
using Luna.IDE.TextDiff;

namespace Luna.IDE.App.ViewModel;

public class DoubleTextDiffViewModel : NotificationObject
{
    private IDoubleTextDiff? _model;

    public IDoubleTextDiff? Model
    {
        get => _model;
        set
        {
            _model = value;
            RaisePropertyChanged(() => Model!);
        }
    }
}
