using Luna.IDE.App.Commands.CodeFileEditor;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.Common;

namespace Luna.IDE.App.ViewModel;

public class CodeFileEditorContextMenuViewModel : NotificationObject
{
    [Inject]
    public ICopyCommand? CopyCommand { get; set; }

    [Inject]
    public IPasteCommand? PasteCommand { get; set; }

    [Inject]
    public ICutCommand? CutCommand { get; set; }

    [Inject]
    public IGotoDeclarationCommand? GotoDeclarationCommand { get; set; }

    [Inject]
    public IFormatCodeCommand? FormatCodeCommand { get; set; }

    [Inject]
    public ICodeFileEditorChangesCommand? ChangesCommand { get; set; }
}
