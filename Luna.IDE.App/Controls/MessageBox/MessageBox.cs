using Luna.IDE.Common;

namespace Luna.IDE.App.Controls.MessageBox;

public class MessageBox : IMessageBox
{
    public MessageBoxResult? Show(string title, string message, MessageBoxButtons buttons)
    {
        var vm = new MessageBoxViewModel();
        vm.Title = title;
        vm.Message = message;
        vm.Buttons = buttons;

        var view = new MessageBoxView(vm);
        vm.Closed += (s, e) => view.Close();

        view.ShowDialog();

        return vm.Result;
    }
}
