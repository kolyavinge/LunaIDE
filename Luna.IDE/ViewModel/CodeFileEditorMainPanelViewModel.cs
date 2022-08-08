using Luna.IDE.Commands.CodeFileEditor;
using Luna.IDE.Infrastructure;
using Luna.IDE.Mvvm;

namespace Luna.IDE.ViewModel;

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
