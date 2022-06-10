using DependencyInjection;
using Luna.IDE.CodeEditor;
using Luna.IDE.Commands;
using Luna.IDE.Infrastructure;
using Luna.IDE.Model;
using Luna.IDE.ViewModel;
using Luna.Infrastructure;
using Luna.Runtime;

namespace Luna.IDE;

public class InjectModule : DependencyInjection.InjectModule
{
    public override void Init(IBindingProvider provider)
    {
        provider.Bind<IFileSystem, FileSystem>().ToSingleton();
        provider.Bind<IOpenFileDialog, OpenFileDialog>();

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
        provider.Bind<IProjectTreeItemOpenCommand, ProjectTreeItemOpenCommand>().ToSingleton();
        provider.Bind<IRunProgramCommand, RunProgramCommand>().ToSingleton();

        provider.Bind<IInterpreter, Interpreter>().ToSingleton();
    }
}
