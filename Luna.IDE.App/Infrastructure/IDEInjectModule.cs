using DependencyInjection;
using Luna.IDE.App.CodeEditor;
using Luna.IDE.App.Commands;
using Luna.IDE.App.Commands.CodeFileEditor;
using Luna.IDE.App.Controls.Tree;
using Luna.IDE.App.Model;
using Luna.IDE.App.ViewModel;

namespace Luna.IDE.App.Infrastructure;

public class IDEInjectModule : InjectModule
{
    public override void Init(IBindingProvider provider)
    {
        provider.Bind<IOpenFileDialog, OpenFileDialog>();

        provider.Bind<TreeViewModel, TreeViewModel>();

        provider.Bind<ICodeProviderFactory, CodeProviderFactory>().ToSingleton();

        provider.Bind<IProjectItemEditorFactory, ProjectItemEditorFactory>().ToSingleton();

        provider.Bind<IEnvironmentWindowsManager, EnvironmentWindowsManager>().ToSingleton();
        provider.Bind<EnvironmentWindowsViewModel, EnvironmentWindowsViewModel>().ToSingleton();

        provider.Bind<IProjectRepository, ProjectRepository>().ToSingleton();

        provider.Bind<IProjectChanges, ProjectChanges>().ToSingleton();
        provider.Bind<ProjectChangesViewModel, ProjectChangesViewModel>().ToSingleton();
        provider.Bind<IProjectExplorer, ProjectExplorer>().ToSingleton();
        provider.Bind<ProjectExplorerViewModel, ProjectExplorerViewModel>().ToSingleton();
        provider.Bind<AutoComplete, AutoComplete>();
        provider.Bind<AutoCompleteViewModel, AutoCompleteViewModel>();
        provider.Bind<CodeFileEditorMainPanelViewModel, CodeFileEditorMainPanelViewModel>();

        provider.Bind<IOutputArea, OutputArea>().ToSingleton();
        provider.Bind<OutputAreaViewModel, OutputAreaViewModel>().ToSingleton();

        provider.Bind<IOutputConsole, OutputConsole>().ToSingleton();
        provider.Bind<OutputConsoleViewModel, OutputConsoleViewModel>().ToSingleton();

        provider.Bind<MainViewModel, MainViewModel>().ToSingleton();

        provider.Bind<IOpenProjectCommand, OpenProjectCommand>().ToSingleton();
        provider.Bind<IProjectItemOpenCommand, ProjectItemOpenCommand>().ToSingleton();
        provider.Bind<ICodeElementNavigateCommand, CodeElementNavigateCommand>().ToSingleton();
        provider.Bind<IProjectExplorerItemOpenCommand, ProjectExplorerItemOpenCommand>().ToSingleton();
        provider.Bind<IGotoDeclarationCommand, GotoDeclarationCommand>().ToSingleton();
        provider.Bind<IRunProgramCommand, RunProgramCommand>().ToSingleton();
        provider.Bind<IUndoCommand, UndoCommand>().ToSingleton();
        provider.Bind<IRedoCommand, RedoCommand>().ToSingleton();
        provider.Bind<IToUpperCaseCommand, ToUpperCaseCommand>().ToSingleton();
        provider.Bind<IToLowerCaseCommand, ToLowerCaseCommand>().ToSingleton();

        provider.Bind<IMainWindowClosedCommand, MainWindowClosedCommand>().ToSingleton();
    }
}
