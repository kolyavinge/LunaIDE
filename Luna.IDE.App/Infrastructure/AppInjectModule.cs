using DependencyInjection;
using Luna.IDE.App.Commands;
using Luna.IDE.App.Commands.CodeFileEditor;
using Luna.IDE.App.Controls.MessageBox;
using Luna.IDE.App.Factories;
using Luna.IDE.App.Media;
using Luna.IDE.App.ViewModel;
using Luna.IDE.Common;
using Luna.IDE.ProjectExploration;

namespace Luna.IDE.App.Infrastructure;

public class AppInjectModule : InjectModule
{
    public override void Init(IBindingProvider provider)
    {
        provider.Bind<IOpenFileDialog, OpenFileDialog>();
        provider.Bind<IMessageBox, MessageBox>().ToSingleton();
        provider.Bind<IImageCollection, ImageCollection>().ToSingleton();

        provider.Bind<IEnvironmentWindowsFactory, EnvironmentWindowsFactory>().ToSingleton();
        provider.Bind<IProjectItemEditorFactory, ProjectItemEditorFactory>().ToSingleton();
        provider.Bind<EnvironmentWindowsViewModel, EnvironmentWindowsViewModel>().ToSingleton();
        provider.Bind<RecentProjectsViewModel, RecentProjectsViewModel>().ToSingleton();
        provider.Bind<ProjectExplorerViewModel, ProjectExplorerViewModel>().ToSingleton();
        provider.Bind<AutoCompleteViewModel, AutoCompleteViewModel>();
        provider.Bind<CodeFileEditorMainPanelViewModel, CodeFileEditorMainPanelViewModel>();
        provider.Bind<CodeFileEditorContextMenuViewModel, CodeFileEditorContextMenuViewModel>();
        provider.Bind<ProjectChangesViewModel, ProjectChangesViewModel>().ToSingleton();
        provider.Bind<SingleTextDiffViewModel, SingleTextDiffViewModel>();
        provider.Bind<DoubleTextDiffViewModel, DoubleTextDiffViewModel>();
        provider.Bind<OutputAreaViewModel, OutputAreaViewModel>().ToSingleton();
        provider.Bind<OutputConsoleViewModel, OutputConsoleViewModel>().ToSingleton();
        provider.Bind<MainViewModel, MainViewModel>().ToSingleton();

        provider.Bind<IOpenProjectCommand, OpenProjectCommand>().ToSingleton();
        provider.Bind<IProjectItemOpenCommand, ProjectItemOpenCommand>().ToSingleton();
        provider.Bind<IOpenProjectHistoryCommand, OpenProjectHistoryCommand>().ToSingleton();
        provider.Bind<ICodeElementNavigateCommand, CodeElementNavigateCommand>().ToSingleton();
        provider.Bind<IProjectExplorerItemOpenCommand, ProjectExplorerItemOpenCommand>().ToSingleton();
        provider.Bind<IVersionedFilesChangesCommand, VersionedFilesChangesCommand>().ToSingleton();
        provider.Bind<IGotoDeclarationCommand, GotoDeclarationCommand>().ToSingleton();
        provider.Bind<IRunProgramCommand, RunProgramCommand>().ToSingleton();
        provider.Bind<IUndoCommand, UndoCommand>().ToSingleton();
        provider.Bind<IRedoCommand, RedoCommand>().ToSingleton();
        provider.Bind<ICopyCommand, CopyCommand>().ToSingleton();
        provider.Bind<IPasteCommand, PasteCommand>().ToSingleton();
        provider.Bind<ICutCommand, CutCommand>().ToSingleton();
        provider.Bind<IToUpperCaseCommand, ToUpperCaseCommand>().ToSingleton();
        provider.Bind<IToLowerCaseCommand, ToLowerCaseCommand>().ToSingleton();
        provider.Bind<IFormatCodeCommand, FormatCodeCommand>().ToSingleton();
        provider.Bind<ICodeFileEditorChangesCommand, CodeFileEditorChangesCommand>().ToSingleton();
        provider.Bind<IMainWindowClosedCommand, MainWindowClosedCommand>().ToSingleton();
    }
}
