using System.Windows;
using System.Windows.Controls;
using Luna.IDE.Infrastructure;
using Luna.IDE.Model;
using Luna.IDE.ViewModel;

namespace Luna.IDE.View;

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
