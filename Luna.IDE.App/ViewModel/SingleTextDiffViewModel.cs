using Luna.IDE.Common;
using Luna.IDE.TextDiff;

namespace Luna.IDE.App.ViewModel;

public class SingleTextDiffViewModel : NotificationObject
{
    private ISingleTextDiff? _model;

    public ISingleTextDiff? Model
    {
        get => _model;
        set
        {
            _model = value;
            RaisePropertyChanged(() => Model!);
        }
    }
}
