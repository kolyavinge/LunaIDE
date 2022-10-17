using System.Windows;

namespace Luna.IDE.App.Controls.MessageBox;

public partial class MessageBoxView : Window
{
    public MessageBoxView()
    {
        InitializeComponent();
    }

    public MessageBoxView(MessageBoxViewModel vm) : this()
    {
        DataContext = vm;
    }
}
