using System.Windows.Input;
using Luna.IDE.App.Mvvm;
using Luna.IDE.Common;

namespace Luna.IDE.App.Controls.MessageBox;

public class MessageBoxViewModel : NotificationObject
{
	public event EventHandler? Closed;

	public string Title { get; set; }

	public string Message { get; set; }

	public MessageBoxButtons Buttons { get; set; }

	public ICommand YesCommand { get; }

	public ICommand NoCommand { get; }

	public MessageBoxResult? Result { get; private set; }

	public MessageBoxViewModel()
	{
		Title = "";
		Message = "";
		YesCommand = new ActionCommand(Yes);
		NoCommand = new ActionCommand(No);
	}

	public void Close()
	{
		Closed?.Invoke(this, EventArgs.Empty);
	}

	private void Yes()
	{
		Result = MessageBoxResult.Yes;
		Close();
	}

	private void No()
	{
		Result = MessageBoxResult.No;
		Close();
	}
}
