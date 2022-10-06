using System.Windows;
using System.Windows.Controls;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.Model;
using Luna.IDE.App.ViewModel;

namespace Luna.IDE.App.View;

public partial class CodeFileEditorMainPanelView : UserControl
{
    public ICodeFileEditor CodeFileEditor
    {
        get { return (ICodeFileEditor)GetValue(CodeFileEditorProperty); }
        set { SetValue(CodeFileEditorProperty, value); }
    }

    public static readonly DependencyProperty CodeFileEditorProperty =
        DependencyProperty.Register("CodeFileEditor", typeof(ICodeFileEditor), typeof(CodeFileEditorMainPanelView));

    public CodeFileEditorMainPanelView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<CodeFileEditorMainPanelViewModel>();
    }
}
