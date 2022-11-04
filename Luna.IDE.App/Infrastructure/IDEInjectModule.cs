using DependencyInjection;
using Luna.IDE.App.Commands;
using Luna.IDE.App.Commands.CodeFileEditor;
using Luna.IDE.App.Controls.MessageBox;
using Luna.IDE.App.Factories;
using Luna.IDE.App.Media;
using Luna.IDE.App.ViewModel;
using Luna.IDE.AutoCompletion;
using Luna.IDE.CodeEditing;
using Luna.IDE.Common;
using Luna.IDE.Outputing;
using Luna.IDE.ProjectChanging;
using Luna.IDE.ProjectExploration;
using Luna.IDE.Versioning;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;

namespace Luna.IDE.App.Infrastructure;

public class IDEInjectModule : InjectModule
{
    public override void Init(IBindingProvider provider)
    {
        provider.Bind<IOpenFileDialog, OpenFileDialog>();
        provider.Bind<IMessageBox, MessageBox>().ToSingleton();
        provider.Bind<IImageCollection, ImageCollection>().ToSingleton();

        provider.Bind<ISingleFileCodeModelBuilder, SingleFileCodeModelBuilder>().ToSingleton();
        provider.Bind<ICodeProviderFactory, CodeProviderFactory>().ToSingleton();
        provider.Bind<IEnvironmentWindowsFactory, EnvironmentWindowsFactory>().ToSingleton();
        provider.Bind<IProjectItemEditorFactory, ProjectItemEditorFactory>().ToSingleton();
        provider.Bind<IEnvironmentWindowsManager, EnvironmentWindowsManager>().ToSingleton();
        provider.Bind<EnvironmentWindowsViewModel, EnvironmentWindowsViewModel>().ToSingleton();
        provider.Bind<ICodeEditorSaver, CodeEditorSaver>().ToSingleton();
        provider.Bind<ICodeEditorUndoChangesLogic, CodeEditorUndoChangesLogic>().ToSingleton();
        provider.Bind<IVersionControlRepositoryFactory, VersionControlRepositoryFactory>().ToSingleton();
        provider.Bind<IProjectRepository, ProjectRepository>().ToSingleton();

        provider.Bind<IProjectLoader, ProjectLoader>().ToSingleton();
        provider.Bind<ISelectedProject>().ToMethod(provider => provider.Resolve<IProjectLoader>());
        provider.Bind<IProjectExplorer, ProjectExplorer>().ToSingleton();
        provider.Bind<ProjectExplorerViewModel, ProjectExplorerViewModel>().ToSingleton();
        provider.Bind<AutoComplete, AutoComplete>();
        provider.Bind<AutoCompleteViewModel, AutoCompleteViewModel>();
        provider.Bind<CodeFileEditorMainPanelViewModel, CodeFileEditorMainPanelViewModel>();
        provider.Bind<IProjectChanges, ProjectChanges>().ToSingleton();
        provider.Bind<ProjectChangesViewModel, ProjectChangesViewModel>().ToSingleton();
        provider.Bind<IOutputArea, OutputArea>().ToSingleton();
        provider.Bind<OutputAreaViewModel, OutputAreaViewModel>().ToSingleton();
        provider.Bind<IOutputConsole, OutputConsole>().ToSingleton();
        provider.Bind<OutputConsoleViewModel, OutputConsoleViewModel>().ToSingleton();
        provider.Bind<MainViewModel, MainViewModel>().ToSingleton();

        provider.Bind<IOpenProjectCommand, OpenProjectCommand>().ToSingleton();
        provider.Bind<IProjectItemOpenCommand, ProjectItemOpenCommand>().ToSingleton();
        provider.Bind<IOpenProjectHistoryCommand, OpenProjectHistoryCommand>().ToSingleton();
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
