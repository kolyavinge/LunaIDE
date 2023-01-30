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
using Luna.IDE.Configuration;
using Luna.IDE.Outputing;
using Luna.IDE.ProjectChanging;
using Luna.IDE.ProjectExploration;
using Luna.IDE.TextDiff;
using Luna.IDE.Versioning;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.Infrastructure;

public class IDEInjectModule : InjectModule
{
    public override void Init(IBindingProvider provider)
    {
        provider.Bind<IDateTimeProvider, DateTimeProvider>();
        provider.Bind<IOpenFileDialog, OpenFileDialog>();
        provider.Bind<IMessageBox, MessageBox>().ToSingleton();
        provider.Bind<IImageCollection, ImageCollection>().ToSingleton();

        provider.Bind<ICodeProviderFactory, CodeProviderFactory>().ToSingleton();
        provider.Bind<IEnvironmentWindowsFactory, EnvironmentWindowsFactory>().ToSingleton();
        provider.Bind<IProjectItemEditorFactory, ProjectItemEditorFactory>().ToSingleton();
        provider.Bind<IEnvironmentWindowsManager, EnvironmentWindowsManager>().ToSingleton();
        provider.Bind<EnvironmentWindowsViewModel, EnvironmentWindowsViewModel>().ToSingleton();
        provider.Bind<ICodeEditorSaver, CodeEditorSaver>().ToSingleton();
        provider.Bind<ICodeEditorUndoChangesLogic, CodeEditorUndoChangesLogic>().ToSingleton();
        provider.Bind<IVersionControlRepositoryFactory, VersionControlRepositoryFactory>().ToSingleton();
        provider.Bind<IProjectRepository, ProjectRepository>().ToSingleton();
        provider.Bind<ITextDiffEngine, TextDiffEngine>().ToSingleton();
        provider.Bind<ITextDiffCodeProviderFactory, TextDiffCodeProviderFactory>().ToSingleton();

        provider.Bind<IProjectLoader, ProjectLoader>().ToSingleton();
        provider.Bind<ISelectedProject>().ToMethod(provider => provider.Resolve<IProjectLoader>());
        provider.Bind<IProjectExplorer, ProjectExplorer>().ToSingleton();
        provider.Bind<IConfigStorage, ConfigStorage>().ToSingleton();
        provider.Bind<ILastOpenedProjectFiles, LastOpenedProjectFiles>().ToSingleton();
        provider.Bind<IRecentProjects, RecentProjects>().ToSingleton();
        provider.Bind<RecentProjectsViewModel, RecentProjectsViewModel>().ToSingleton();
        provider.Bind<ProjectExplorerViewModel, ProjectExplorerViewModel>().ToSingleton();
        provider.Bind<IAutoComplete, AutoComplete>();
        provider.Bind<AutoCompleteViewModel, AutoCompleteViewModel>();
        provider.Bind<CodeFileEditorMainPanelViewModel, CodeFileEditorMainPanelViewModel>();
        provider.Bind<CodeFileEditorContextMenuViewModel, CodeFileEditorContextMenuViewModel>();
        provider.Bind<IProjectChanges, ProjectChanges>().ToSingleton();
        provider.Bind<ProjectChangesViewModel, ProjectChangesViewModel>().ToSingleton();
        provider.Bind<ILinesDecorationProcessor, LinesDecorationProcessor>();
        provider.Bind<ISingleTextDiffGapProcessor, SingleTextDiffGapProcessor>();
        provider.Bind<IDoubleTextDiffGapProcessor, DoubleTextDiffGapProcessor>();
        provider.Bind<IDiffCodeTextBox, DiffCodeTextBox>();
        provider.Bind<ISingleTextDiff, SingleTextDiff>();
        provider.Bind<IDoubleTextDiff, DoubleTextDiff>();
        provider.Bind<SingleTextDiffViewModel, SingleTextDiffViewModel>();
        provider.Bind<DoubleTextDiffViewModel, DoubleTextDiffViewModel>();
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
