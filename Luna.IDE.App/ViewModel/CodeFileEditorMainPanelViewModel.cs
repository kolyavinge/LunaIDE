using Luna.IDE.App.Commands.CodeFileEditor;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.Common;

namespace Luna.IDE.App.ViewModel;

public class CodeFileEditorMainPanelViewModel : NotificationObject
{
    [Inject]
    public IUndoCommand? UndoCommand { get; set; }

    [Inject]
    public IRedoCommand? RedoCommand { get; set; }

    [Inject]
    public IToUpperCaseCommand? ToUpperCaseCommand { get; set; }

    [Inject]
    public IToLowerCaseCommand? ToLowerCaseCommand { get; set; }
}
