using DependencyInjection;
using Luna.IDE.CodeEditor;
using Luna.IDE.Commands;
using Luna.IDE.Controls.Tree;
using Luna.IDE.Model;
using Luna.IDE.ViewModel;

namespace Luna.IDE.Infrastructure;

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

        provider.Bind<IProjectExplorer, ProjectExplorer>().ToSingleton();
        provider.Bind<ProjectExplorerViewModel, ProjectExplorerViewModel>().ToSingleton();
        provider.Bind<ProjectTreeViewModel, ProjectTreeViewModel>().ToSingleton();

        provider.Bind<IOutputArea, OutputArea>().ToSingleton();
        provider.Bind<OutputAreaViewModel, OutputAreaViewModel>().ToSingleton();

        provider.Bind<IOutputConsole, OutputConsole>().ToSingleton();
        provider.Bind<OutputConsoleViewModel, OutputConsoleViewModel>().ToSingleton();

        provider.Bind<MainViewModel, MainViewModel>().ToSingleton();

        provider.Bind<IOpenProjectCommand, OpenProjectCommand>().ToSingleton();
        provider.Bind<IProjectItemOpenCommand, ProjectItemOpenCommand>().ToSingleton();
        provider.Bind<ICodeElementNavigateCommand, CodeElementNavigateCommand>().ToSingleton();
        provider.Bind<IProjectExplorerItemOpenCommand, ProjectExplorerItemOpenCommand>().ToSingleton();
        provider.Bind<IRunProgramCommand, RunProgramCommand>().ToSingleton();
        provider.Bind<IMainWindowClosedCommand, MainWindowClosedCommand>().ToSingleton();
    }
}
