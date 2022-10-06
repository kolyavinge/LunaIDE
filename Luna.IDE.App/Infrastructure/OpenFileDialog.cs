using Microsoft.WindowsAPICodePack.Dialogs;

namespace Luna.IDE.App.Infrastructure;

public interface IOpenFileDialog
{
    bool IsFolderPicker { get; set; }
    string? SelectedPath { get; }
    DialogResult ShowDialog();
}

public enum DialogResult
{
    None,
    Ok,
    Cancel
}

public class OpenFileDialog : IOpenFileDialog
{
    public bool IsFolderPicker { get; set; }

    public string? SelectedPath { get; private set; }

    public DialogResult ShowDialog()
    {
        var dialog = new CommonOpenFileDialog();
        dialog.IsFolderPicker = IsFolderPicker;
        var result = (DialogResult)dialog.ShowDialog();
        if (result == DialogResult.Ok)
        {
            SelectedPath = dialog.FileName;
        }

        return result;
    }
}
