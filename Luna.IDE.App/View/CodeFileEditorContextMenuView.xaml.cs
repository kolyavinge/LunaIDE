using System.Windows;
using System.Windows.Controls;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.App.ViewModel;
using Luna.IDE.CodeEditing;

namespace Luna.IDE.App.View;

public partial class CodeFileEditorContextMenuView : ContextMenu
{
    public ICodeFileEditor CodeFileEditor
    {
        get { return (ICodeFileEditor)GetValue(CodeFileEditorProperty); }
        set { SetValue(CodeFileEditorProperty, value); }
    }

    public static readonly DependencyProperty CodeFileEditorProperty =
        DependencyProperty.Register("CodeFileEditor", typeof(ICodeFileEditor), typeof(CodeFileEditorContextMenuView));

    public CodeFileEditorContextMenuView()
    {
        InitializeComponent();
        DataContext = DependencyContainer.Resolve<CodeFileEditorContextMenuViewModel>();
    }
}
