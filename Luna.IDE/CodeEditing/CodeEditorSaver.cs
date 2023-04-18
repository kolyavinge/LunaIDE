using System.Linq;
using Luna.IDE.WindowsManagement;
using Luna.Utils;

namespace Luna.IDE.CodeEditing;

internal interface ICodeEditorSaver
{
    void SaveOpenedEditors();
}

internal class CodeEditorSaver : ICodeEditorSaver
{
    private readonly IEnvironmentWindowsManager _windowsManager;

    public CodeEditorSaver(IEnvironmentWindowsManager windowsManager)
    {
        _windowsManager = windowsManager;
    }

    public void SaveOpenedEditors()
    {
        _windowsManager.Windows.Where(w => w.Model is ICodeFileEditor).Each(w => ((ICodeFileEditor)w.Model).Save());
    }
}
